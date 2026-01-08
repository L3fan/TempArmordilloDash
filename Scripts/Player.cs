using Godot;
using System;
using System.Collections;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using System.Threading.Tasks;
using Vector2 = Godot.Vector2;

public partial class Player : CharacterBody2D
{
    [Export] public float mass = 1;
    [Export] public float speed = 200f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 3;
    [Export] public float dashForce = 3;
    [Export] public float friction = 500f;
    [Export] public float bounciness = 1.0f;

    private Vector2 direction = new Vector2(1, 0);
    private Vector2 forwardDir = new Vector2(1, 0);
    private Vector2 lastVelocity = Vector2.Zero;

    private float dashCooldown = 0f;
    private float dashCdMax = 1.2f;
    private float dashTime = 0f;
    private bool isDashing = false;
    private Vector2 dashDir = Vector2.Zero;
    private bool dashSlowdown = false;
    private float timeSlow = 3f;
    private bool inSlowDown = false;
    private bool mustRecharge = false;
    private bool dashed = false;

    private bool pressingRight = false;
    private bool pressingLeft = false;
    private bool pressingUp = false;
    private bool pressingDown = false;
    private bool pressingDash = false;

    private float prevRot = 0;

    private bool canControl = true;
    private bool gotHit = false;
    private bool reverse = false;
    private Vector2 respawnPoint = Vector2.Zero;
    private bool fadeToBlack = false;
    private bool fadeOutOfBlack = false;
    private bool respawnOnBlackScreen = false;
    [Export] private CameraMovement playerCamera;

    private Sprite2D sprite;
    private AnimationPlayer animationPlayer;

    private String velocityInfo;

    private float count = 1;
    
    [Export] private Control radialProgress;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
        sprite = GetNode<Sprite2D>("SpritePixel");
        animationPlayer = GetNode<AnimationPlayer>("SpritePixel/AnimationPlayer");
        animationPlayer.Play("Idle");
        GD.Print(Mathf.Max(-2000, -5000));

        GD.Print("Player is ready");
        respawnPoint = Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (fadeToBlack)
        {
            playerCamera.DarkenScreen((float)delta);

            if (playerCamera.ScreenIsBlack())
            {
                fadeToBlack = false;
                fadeOutOfBlack = true;

                if (respawnOnBlackScreen)
                {
                    Position = respawnPoint;
                    Velocity = Vector2.Zero;
                    animationPlayer.SpeedScale = 0;
                    animationPlayer.Play("Idle");
                }
            }
        } else if (fadeOutOfBlack)
        {
            playerCamera.DarkenScreen(-(float)delta);
            if (playerCamera.NoBlackScreen())
            {
                fadeOutOfBlack = false;
                if (respawnOnBlackScreen)
                {
                    GD.Print("Enable Controls again");
                    animationPlayer.SpeedScale = 1;
                    canControl = true;
                    reverse = false;
                    respawnOnBlackScreen = false;
                    gotHit = false;
                }
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (gotHit)
        {
            Velocity = Vector2.Zero;
            lastVelocity = Vector2.Zero;
            return;
        }

        if (dashCooldown > 0f)
        {
            dashCooldown -= (float)delta;
            if (dashCooldown <= 0.25f)
            {
                //recalculate dashCooldown so when it goes from 0.25 to 0, the progress bar goes from 0 to 1.0
                float progress = Mathf.Clamp(((dashCooldown / 0.25f * 2 - 1) * -1 + 1) / 2f, 0, 1);
                radialProgress.Set("progress", progress);
            }
        }

        if (dashCooldown <= 0f)
        {
            dashCooldown = 0f;
            radialProgress.Set("progress", 0);
        }

        if(dashed)
            dashed = !IsOnFloor();

        velocityInfo = "";
        
        CollisionCalc();

        //Gravity
        Velocity += new Vector2(0, 983.4f * mass * (float)delta);
        velocityInfo += "Post Gravity: " + Velocity + "\n";

        //Zero setter for anything between 0.5 and -0.5, cause speed reduction takes too long to go from 0.5 to 0
        if (Velocity.X is < 5f and > -5f)
            Velocity = new Vector2(0, Velocity.Y);

        //Reset Slowdown if timeSlow has reached 0 and set mustRecharge = true, only allowing the player to dash again when it has recharged fully
        if (timeSlow <= 0f)
        {
            mustRecharge = true;
            SetSlowDown(false);
        }

        //recharge timeSlow when not in Slowdown
        if (!inSlowDown && timeSlow < 1.5f)
            timeSlow = Mathf.Min(timeSlow + (float)delta * 0.5f, 1.5f);
        
        
        //allow Dashing when fully recharged after depleting entire timeSlow
        if(timeSlow >= 1.5f && mustRecharge)
            mustRecharge = false;

        InputCalculations((float)delta);

        SlowdownCalc((float)delta);

        DashCalc((float)delta);

        //GD.Print(velocityInfo);

        UpdateSprite();

        lastVelocity = Velocity;

        MoveAndSlide();
    }

    private void CollisionCalc()
    {
        //Get the horizontal Vector or the normalized Vector of the last Velocity
        Vector2 toCompare = (lastVelocity * Vector2.Right).Normalized();
        if (dashCooldown >= dashCdMax/2f)
            toCompare = lastVelocity.Normalized();

        String collisions = "";
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            Vector2 collisionNormal = GetSlideCollision(i).GetNormal();
            float collisionAngle = RotationOfVectors(toCompare, collisionNormal);
            
            //if angle of collision is smaller than 15°, bounce, else slide on surface
            if ((dashCooldown > dashCdMax/2f && collisionAngle > 135f))
            {
                Velocity = lastVelocity.Bounce(collisionNormal) * bounciness;
                
                if (dashDir != Vector2.Zero)
                {
                    dashDir = dashDir.Bounce(collisionNormal);
                }
                
                velocityInfo += "Post Bounce Calc (" + ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name +
                                "): " + Velocity + " " + collisionNormal + "\n";
            }
            else
            {
                Velocity = lastVelocity.Slide(collisionNormal);
                velocityInfo += "Post Slide Calc (" + ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name +
                                "): " + Velocity + " " + collisionNormal + "\n";
            }

            //keep Velocity horizontal on flat ground, prevents unusual hops while rolling from bounce calc, stemming from bad normal calcs
            if (dashCooldown <= dashCdMax/2f)
            {
                float floorAngle = GetSlideCollision(i).GetAngle() * 180 / Mathf.Pi;
                collisions += ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name + ": " + floorAngle +
                              " // " + collisionNormal + "\n";
                if (floorAngle < 15)
                {
                    Velocity *= Vector2.Right;
                }
            }
        }
    }

    private void InputCalculations(float delta)
    {
        if (!canControl)
            return;
        
        if (pressingDash && (dashCooldown == 0f || dashCooldown > dashCdMax-0.2f) && !mustRecharge && !dashed)
        {
            if (timeSlow > 0 && dashCooldown == 0)
            {
                timeSlow -= delta;
                SetSlowDown(true);
            }

            InputDash(delta);
        }
        else
        {
            if(inSlowDown)
                SetSlowDown(false);
            
            InputJumpCalc(delta);

            InputHorizontalCalc(delta);
        }
    }

    private void InputDash(float delta)
    {
        dashDir = Vector2.Zero;

        if (pressingDown)
            dashDir += Vector2.Down;
        if (pressingRight)
            dashDir += Vector2.Right;
        if (pressingLeft)
            dashDir += Vector2.Left;
        if (pressingUp)
            dashDir += Vector2.Up;

        dashDir = dashDir.Normalized() * new Vector2(1.0f, 0.5f);

        if (dashDir != Vector2.Zero && dashCooldown == 0f)
        {
            isDashing = true;
            dashTime = 0.25f;
            dashCooldown = dashCdMax;
            SetSlowDown(false);
            dashed = true;
        }
    }

    private void InputJumpCalc(float delta)
    {
        if (pressingUp && IsOnFloor())
        {
            Velocity *= Vector2.Right;
            Velocity += new Vector2(0, -jumpForce * 200);
        }

        velocityInfo += "Post Jump: " + Velocity + "\n";
    }

    private void InputHorizontalCalc(float delta)
    {
        Vector2 addedForce = Vector2.Zero;

        if (pressingRight)
        {
            float forwardForce = speed * 10 * delta * Mathf.Min(maxSpeed - Velocity.X, maxSpeed) / maxSpeed;
            addedForce = forwardForce * Vector2.Right;
        }

        if (pressingLeft)
        {
            float forwardForce =
                speed * 10 * delta * Mathf.Abs(Mathf.Max(-maxSpeed - Velocity.X, -maxSpeed)) / maxSpeed;
            addedForce = forwardForce * Vector2.Left;
        }

        Velocity += addedForce;
        velocityInfo += "Post Force Add: " + Velocity + "\n";
    }

    private void SlowdownCalc(float delta)
    {
        //Slowdown when no Left/Right Input
        if (!(pressingRight && Velocity.X > 0 || pressingLeft && Velocity.X < 0) && IsOnFloor())
        {
            float slowdownForce = friction * delta;
            //only slow down to 0 X velocity
            slowdownForce = Mathf.Min(slowdownForce, Mathf.Abs(Velocity.X));
            //give it the opposite direction of the X velocity
            if(Velocity.X != 0)
                slowdownForce = slowdownForce * (Velocity.X / Mathf.Abs(Velocity.X));
            Velocity -= new Vector2(slowdownForce, 0);
        }

        velocityInfo += "Post Jump: " + Velocity + "\n";
    }

    private void DashCalc(float delta)
    {
        if (dashSlowdown)
        {
            if (Velocity.Length() >= maxSpeed || dashCooldown >= dashCdMax/2f)
            {
                if(Velocity.Length() > maxSpeed * 0.75f)
                    Velocity *= 0.95f;
                if(Velocity.Y < -1000)
                    Velocity *= new Vector2(1f, 0.9f);
            } else
            {
                if(lastVelocity.Length() > maxSpeed)
                    Velocity = Velocity.Normalized() * maxSpeed;
                dashSlowdown = false;
            }
        }

        if (isDashing)
        {
            if (!dashSlowdown)
            {
                float force = dashForce * 100000 * delta;
                Velocity *= 0.1f;
                Velocity += force * dashDir;
                isDashing = false;
                dashSlowdown = true;
                dashTime = 0.25f;
            }

            if (dashTime > 0f)
                dashTime -= delta;
            if (dashTime <= 0f)
            {
                dashTime = 0f;
                isDashing = false;
            }
        }
    }

    private bool AboutSameVector(Vector2 vec1, Vector2 vec2, float errorAngle)
    {
        float radAngle = Mathf.Acos(vec1.Dot(vec2) / (vec1.Length() * vec2.Length()));
        float angle = radAngle * 180f / Mathf.Pi;
        return angle <= errorAngle;
    }

    private Vector2 LeftOrRightDir(Vector2 vec)
    {
        vec.Normalized();
        if (vec.X > 0)
            return new Vector2(1, 0);
        if (vec.X < 0)
            return new Vector2(-1, 0);
        return direction;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("right"))
            pressingRight = true;
        else if (@event.IsActionReleased("right"))
            pressingRight = false;

        if (@event.IsActionPressed("left"))
            pressingLeft = true;
        else if (@event.IsActionReleased("left"))
            pressingLeft = false;

        if (@event.IsActionPressed("up"))
            pressingUp = true;
        else if (@event.IsActionReleased("up"))
            pressingUp = false;

        if (@event.IsActionPressed("down"))
            pressingDown = true;
        else if (@event.IsActionReleased("down"))
            pressingDown = false;

        if (@event.IsActionPressed("dash"))
            pressingDash = true;
        else if (@event.IsActionReleased("dash"))
            pressingDash = false;
        
        if(@event.IsActionPressed("playDeath"))
            animationPlayer.Play("Death");
    }

    public void SetDirection(Vector2 vec)
    {
        direction = vec.Normalized();
    }

    public float RotationOfVectors(Vector2 vec1, Vector2 vec2)
    {
        float radiansAngle = Mathf.Acos(vec1.Dot(vec2) / (vec1.Length() * vec2.Length()));
        return radiansAngle * 180 / Mathf.Pi;
    }

    private void UpdateSprite()
    {
        //GD.Print("Press Right: " + pressingRight + " // Press Left: " + pressingLeft);
        if (Velocity.X > 0 && sprite.Scale.X < 0)
            sprite.Scale = new Vector2(Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);
        else if (Velocity.X < 0 && sprite.Scale.X > 0)
            sprite.Scale = new Vector2(-Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);

        if (reverse)
            sprite.Scale *= new Vector2(-1, 1);

        float givenForce;
        if (IsOnFloor())
            givenForce = (Velocity * Vector2.Right).Length();
        else
            givenForce = Velocity.Length();

        if (givenForce > 0)
        {
            if (!isDashing)
            {
                if (Mathf.Abs(lastVelocity.X) <= 0)
                    animationPlayer.Play("Roll");
            }
            else
            {
                if (animationPlayer.CurrentAnimation != "Roll")
                    animationPlayer.Play("Roll");
            }

            switch (givenForce)
            {
                case var n when (n < 800):
                    GetNode<Sprite2D>("Sprite").Frame = 0;
                    break;

                case var n when (n >= 800 && n < 1600):
                    GetNode<Sprite2D>("Sprite").Frame = 3;
                    break;

                case var n when (n >= 1600 && n < 3200):
                    GetNode<Sprite2D>("Sprite").Frame = 4;
                    break;

                case var n when (n >= 3200):
                    GetNode<Sprite2D>("Sprite").Frame = 5;
                    break;
            }

            animationPlayer.SpeedScale = givenForce / maxSpeed * 10;
        }
        else if (animationPlayer.CurrentAnimation != "Idle")
        {
            animationPlayer.Play("Idle");
            animationPlayer.SpeedScale = 1;
        }
    }

    private float ValueFurtherFromZero(float val1, float val2)
    {
        if (Mathf.Abs(val1) > Mathf.Abs(val2))
            return val1;
        return val2;
    }

    public float GetDashCooldown()
    {
        return dashCooldown;
    }

    private void SetSlowDown(bool slowDown)
    {
        if (slowDown)
        {
            Engine.TimeScale = 0.5f;
        }
        else
        {
            Engine.TimeScale = 1f;
        }

        inSlowDown = slowDown;
    }

    public bool IsInSlowDown()
    {
        return inSlowDown;
    }

    public bool IsDashing()
    {
        return dashCooldown >= dashCdMax/2f;
    }

    public Sprite2D GetSprite()
    {
        return sprite;
    }

    public Vector2 GetLastVelocity()
    {
        return lastVelocity;
    }

    public void TakeDamage(Node2D source)
    {
        //Do not get hit while already in damage routine
        if (gotHit)
            return;
        
        gotHit = true;
        animationPlayer.SpeedScale = 0;
        StartCoroutine(PlayDeathAnim());
    }

    IEnumerable PlayDeathAnim()
    {
        while (count > 0)
        {
            count -= (float)GetProcessDeltaTime();
            GD.Print("count: " + count);
            yield return null;
        }

        count = 1;
        reverse = true;
        canControl = false;
        //GD.Print("X Distance between damage source and player: " + (source.Position.X - Position.X));
        Velocity = Vector2.Zero;
        animationPlayer.SpeedScale = 1;
        animationPlayer.Play("Death");
        //GD.Print("Velocity: " + Velocity);
        Respawn();
    }

    public static async void StartCoroutine(IEnumerable coroutine)
    {
        var mainLoopTree = Engine.GetMainLoop();
        foreach (var _ in coroutine)
        {
            await mainLoopTree.ToSignal(mainLoopTree, SceneTree.SignalName.ProcessFrame);
        }
    }

    private void Respawn()
    {
        fadeToBlack = true;
        respawnOnBlackScreen = true;
    }
}