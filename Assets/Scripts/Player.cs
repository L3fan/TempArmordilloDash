using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using Vector2 = Godot.Vector2;

public partial class Player : RigidBody2D
{
    [Export] public float mass = 1;
    [Export] public float speed = 200f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 3;
    [Export] public float dashForce = 3;
    [Export] public float friction = 50f;
    [Export] public float bounciness = 1.0f;

    private Vector2 direction = new Vector2(1, 0);
    private Vector2 forwardDir = new Vector2(1, 0);
    private Vector2 lastVelocity = Vector2.Zero;
    private List<Object> lastColliders = new List<Object>();
    private CollisionShape2D collisionShape;
    [Export] private bool isOnFloor = false;

    private float dashCooldown = 0f;
    private float dashCdMax = 1.2f;
    private bool isDashing = false;
    private bool startDash = false;
    private Vector2 dashDir = Vector2.Zero;
    private bool dashSlowdown = false;
    private bool justLanded = false;
    private float slowdownMult = 3;
    private float timeSlow = 3f;
    private bool inSlowDown = false;
    private bool mustRecharge = false;
    private bool dashed = false;

    private bool pressingRight = false;
    private bool pressingLeft = false;
    private bool pressingUp = false;
    private bool pressingDown = false;
    private bool pressingDash = false;
    private bool letGoOfDash = false;

    private float prevRot = 0;

    private bool canControl = true;
    private Vector2 respawnPoint = Vector2.Zero;

    private Sprite2D sprite;
    private AnimationPlayer animationPlayer;

    private String velocityInfo;

    private float count = 1;

    [Export] private Control radialProgress;

    private EnvState envState = EnvState.DEFAULT;

    public Controls currentControls;

    private float drag = 1.0f;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ProcessMode = ProcessModeEnum.Pausable;
        sprite = GetNode<Sprite2D>("SpritePixel");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        animationPlayer = GetNode<AnimationPlayer>("SpritePixel/AnimationPlayer");
        animationPlayer.Play("Idle");

        respawnPoint = Position;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        base._IntegrateForces(state);

        float delta = state.Step;

        isOnFloor = GetFloorState(state);

        velocityInfo = "";
        SlowdownCalc(delta);
        
        DashCooldownCalc(delta);

        //CollisionCalc((float)delta);

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
        if (timeSlow >= 1.5f && mustRecharge)
            mustRecharge = false;

        InputCalculations((float)delta);

        SlowdownCalc((float)delta);

        DashCalc((float)delta);

        //GD.Print(velocityInfo);

        UpdateSprite();

        lastVelocity = LinearVelocity;
        
    }

    private bool GetFloorState(PhysicsDirectBodyState2D state)
    {
        for (int i = 0; i < state.GetContactCount(); i++)
        {
            Vector2 contactPosition = state.GetContactColliderPosition(i);
            if (contactPosition.Y > Position.Y)
                return true;
        }

        return false;
    }

    private void DashCooldownCalc(float delta)
    {
        if (dashCooldown > 0f)
        {
            dashCooldown -= delta;
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

        if (dashed)
            dashed = !isOnFloor;
    }

    private void InputCalculations(float delta)
    {
        if (!canControl)
            return;

        if (!dashed && dashCooldown == 0f && !mustRecharge)
        {
            if (pressingDash)
            {
                if (timeSlow > 0 && dashCooldown == 0f)
                {
                    timeSlow -= delta;
                    SetSlowDown(true);
                }
            }
            else
            {
                if (inSlowDown)
                    SetSlowDown(false);
            }

            if (letGoOfDash && !isOnFloor)
            {
                letGoOfDash = false;
                InputDash(delta);
            }
        }

        InputJumpCalc(delta);

        InputHorizontalCalc(delta);
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

        dashDir = dashDir.Normalized();

        if (dashDir != Vector2.Zero)
        {
            startDash = true;
            dashCooldown = dashCdMax;
            SetSlowDown(false);
            dashed = true;
            PhysicsMaterialOverride.Bounce = 1;
        }
    }

    private void InputJumpCalc(float delta)
    {
        if (pressingUp && isOnFloor)
        {
            LinearVelocity *= Vector2.Right;
            LinearVelocity += new Vector2(0, -jumpForce * 200);
        }

        velocityInfo += "Post Jump: " + LinearVelocity + "\n";
    }

    private void InputHorizontalCalc(float delta)
    {
        Vector2 addedForce = Vector2.Zero;

        if (pressingRight)
        {
            float forwardForce = speed * 10 * delta * Mathf.Min(maxSpeed - LinearVelocity.X, maxSpeed) / maxSpeed;
            addedForce = forwardForce * Vector2.Right;
        }

        if (pressingLeft)
        {
            float forwardForce =
                speed * 10 * delta * Mathf.Abs(Mathf.Max(-maxSpeed - LinearVelocity.X, -maxSpeed)) / maxSpeed;
            addedForce = forwardForce * Vector2.Left;
        }

        LinearVelocity += addedForce;
        velocityInfo += "Post Force Add: " + LinearVelocity + "\n";

        if (pressingLeft || pressingRight)
            PhysicsMaterialOverride.Friction = Mathf.Max(0.5f - (LinearVelocity.Length() / 500f), 0);
        else
            PhysicsMaterialOverride.Friction = 0.5f;
    }

    private void SlowdownCalc(float delta)
    {
        //Slowdown when no left/right input and on floor
        if ((!pressingRight && LinearVelocity.X > 0 || !pressingLeft && LinearVelocity.X < 0) && isOnFloor)
        {
            float slowdownForce = friction * delta;

            if (justLanded)
            {
                slowdownForce += friction * delta * 0.5f;
                justLanded = false;
            }

            //only slow down to 0 velocity on X axis
            slowdownForce = Mathf.Min(slowdownForce, Mathf.Abs(LinearVelocity.X));

            //give it the opposite direction of the X velocity
            if (LinearVelocity.X != 0)
                slowdownForce *= (LinearVelocity.X / Mathf.Abs(LinearVelocity.X));
            LinearVelocity -= new Vector2(slowdownForce, 0);
        }

        LinearVelocity *= drag;

        if (isOnFloor && justLanded)
            justLanded = false;

        if (!isOnFloor)
            justLanded = true;

        velocityInfo += "Post Slowdown: " + LinearVelocity + "\n";
    }

    private void DashCalc(float delta)
    {
        if (dashSlowdown)
        {
            if (LinearVelocity.Length() >= maxSpeed || dashCooldown >= dashCdMax / 2f)
            {
                if (LinearVelocity.Length() > maxSpeed)
                    LinearVelocity /= 1.95f;
                if (LinearVelocity.Y < -1000)
                    LinearVelocity *= new Vector2(1f, 0.95f);
            }
            else
            {
                dashSlowdown = false;
                isDashing = false;
                PhysicsMaterialOverride.Bounce = 0;
            }
        }

        if (startDash)
        {
            if (!dashSlowdown)
            {
                float force = Mathf.Max(dashForce * 1000, LinearVelocity.Length());
                LinearVelocity *= 0.5f;
                LinearVelocity += force * dashDir;
                startDash = false;
                isDashing = true;
                dashSlowdown = true;
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
        {
            letGoOfDash = true;
            pressingDash = false;
        }

        if (@event.IsActionPressed("playDeath"))
            animationPlayer.Play("Death");
    }

    public void SetDirection(Vector2 vec)
    {
        direction = vec.Normalized();
    }

    public float RotationOfVectors(Vector2 vec1, Vector2 vec2)
    {
        float radiansAngle = Mathf.Acos(vec1.Dot(vec2) / (vec1.Length() * vec2.Length()));
        return Mathf.Abs(radiansAngle * 180 / Mathf.Pi);
    }

    private void UpdateSprite()
    {
        //turn sprite around depending on velocity
        if (LinearVelocity.X > 0 && sprite.Scale.X < 0)
            sprite.Scale = new Vector2(Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);
        else if (LinearVelocity.X < 0 && sprite.Scale.X > 0)
            sprite.Scale = new Vector2(-Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);

        //when on floor, only consider the X value, otherwise use the length of the LinearVelocity to determine sprite animation
        float givenForce = LinearVelocity.Length();
        if (isOnFloor)
            givenForce = LinearVelocity.X;

        switch (givenForce)
        {
            case var n when n == 0:
                animationPlayer.Play("Idle");
                break;
            case var n when n < 1600:
                animationPlayer.Play("Roll");
                break;

            case var n when (n >= 1600):
                animationPlayer.Play("Roll2");
                break;
        }
        
        animationPlayer.SpeedScale = givenForce / maxSpeed * 10;
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
        return isDashing;
    }

    public Sprite2D GetSprite()
    {
        return sprite;
    }

    public Vector2 GetLastVelocity()
    {
        return lastVelocity;
    }

    public static async void StartCoroutine(IEnumerable coroutine)
    {
        var mainLoopTree = Engine.GetMainLoop();
        foreach (var _ in coroutine)
        {
            await mainLoopTree.ToSignal(mainLoopTree, SceneTree.SignalName.ProcessFrame);
        }
    }

    public void SetEnvState(EnvState state)
    {
        envState = state;
        UpdateEnvState();
    }

    private void UpdateEnvState()
    {
        switch (envState)
        {
            case EnvState.DEFAULT:
                drag = 1.0f;
                jumpForce /= 3f;                
                break;
            case EnvState.WATER:
                drag = 0.975f;
                jumpForce *= 3f;
                break;
        }
    }
}

public enum EnvState
{
    DEFAULT,
    WATER
}