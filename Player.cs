using Godot;
using System;
using System.Diagnostics;
using System.Net;

public partial class Player : RigidBody2D
{
    [Export] public float speed = 10f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 600;
    [Export] public float slowDown = 0.05f;
    [Export] public float verticalBounceReduction = 0.25f;
    [Export] public Control arrow;
    private Vector2 direction = new Vector2(1, 1);
    private bool pressingForward = false;
    private bool pressingJump = false;
    private bool bounced = false;
    private Vector2 lastVelocity = Vector2.Zero;
    private float prevRot = 0;
    private Node2D spriteParent;
    private Sprite2D sprite;
    private AnimationPlayer animationPlayer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Player is ready");
        /*Vector2 vecA = new Vector2(0, 10);
        Vector2 vecB = new Vector2(0, -1);
        Vector2 projNV = vecA.Normalized() * vecB / Mathf.Pow(vecB.Length(), 2) * vecB;
        Vector2 c = vecA.Normalized() - projNV;
        GD.Print(vecA.Slide(projNV + c));*/
        GD.Print("Scale: " + Scale);
        animationPlayer = GetNode<AnimationPlayer>("Sprite Parent/Sprite/AnimationPlayer");
        animationPlayer.Play("Idle");
        ContactMonitor = true;
        MaxContactsReported = 4;
        spriteParent = GetNode<Node2D>("Sprite Parent");
        sprite = GetNode<Sprite2D>("Sprite Parent/Sprite");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        /*if (lastVelocity != Vector2.Zero)
        {
            isOnSlope = false;
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                Vector2 collisionNormal = GetSlideCollision(i).GetNormal();
                //GD.Print("Rotation of " + lastVelocity + " and " + collisionNormal + " = " + RotationOfVectors(lastVelocity, collisionNormal));
                float collisionAngle = RotationOfVectors(direction, collisionNormal);
                if (collisionAngle > 150f)
                {
                    LinearVelocity = lastVelocity.Bounce(collisionNormal);
                    //GD.Print(LinearVelocity);
                    direction.X *= -1;
                }
                else
                {
                    LinearVelocity = lastVelocity.Slide(collisionNormal);
                }

                if (collisionAngle < 150)
                    isOnSlope = true;
            }
        }

        float slopeMod = 1.0f;
        if (isOnSlope)
            slopeMod = 0.1f;
        
        if(!IsOnFloor())
            LinearVelocity += new Vector2(0, 983.4f * mass * slopeMod * (float)delta);

        if (LinearVelocity.X < 0.5f && LinearVelocity.X > -0.5f)
            LinearVelocity = new Vector2(0, LinearVelocity.Y);*/


        //MoveAndSlide();
    }

    public override void _IntegrateForces(PhysicsDirectBodyState2D state)
    {
        bool isOnFloor = state.GetContactCount() > 0 && (int)state.GetContactColliderPosition(0).Y >= (int)GlobalPosition.Y;
        GD.Print(pressingJump + " " + isOnFloor);

        if (pressingJump && isOnFloor)
            LinearVelocity += new Vector2(0, -jumpForce*5 / Mass);
        
        //Input
        float forwardForce = speed * (maxSpeed - Mathf.Abs(LinearVelocity.X)) / maxSpeed;
        
        if (pressingForward)
            LinearVelocity += forwardForce * direction;

        if (!pressingForward && isOnFloor)
            LinearVelocity -= new Vector2(slowDown * LinearVelocity.X, 0);
        
        //GD.Print("Velocity: " + LinearVelocity + " // last Vel: " + lastVelocity);
        if ((lastVelocity.X > 0 && LinearVelocity.X <= 0) || (lastVelocity.X < 0 && LinearVelocity.X >= 0))
        {
            direction *= new Vector2(-1, 1);
            spriteParent.Scale *= new Vector2(-1, 1);
        }

        if (lastVelocity.Y > 0 && LinearVelocity.Y <= 0)
        {
            LinearVelocity *= new Vector2(1, verticalBounceReduction);
        }

        UpdateSprite();
        
        if(Mathf.IsEqualApprox(LinearVelocity.Y, 0, 0.1f))
            LinearVelocity = new Vector2(LinearVelocity.X, 0);
        
        lastVelocity = LinearVelocity;
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
        if (@event.IsActionPressed("forward"))
            pressingForward = true;
        else if (@event.IsActionReleased("forward"))
            pressingForward = false;

        if (@event.IsActionPressed("jump"))
            pressingJump = true;
        else if (@event.IsActionReleased("jump"))
            pressingJump = false;

        if (@event.IsActionPressed("left"))
            GD.Print("Weird Thing Detected!");
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
        if (Mathf.Abs(LinearVelocity.X) > 0)
        {
            if(Mathf.Abs(lastVelocity.X) <= 0)
                animationPlayer.Play("Roll");

            if (animationPlayer.SpeedScale >= 2 && sprite.Frame != 4)
                sprite.Frame = 4;
            
            if (animationPlayer.SpeedScale < 2 && sprite.Frame != 0)
                sprite.Frame = 0;
        }
        else if(Mathf.Abs(lastVelocity.X) > 0)
        {
            animationPlayer.Play("Idle");
            animationPlayer.SpeedScale = 1;
        }
        
        if(Mathf.Abs(LinearVelocity.X) > 0)
            animationPlayer.SpeedScale = Mathf.Abs(LinearVelocity.X) / maxSpeed;
    }
}