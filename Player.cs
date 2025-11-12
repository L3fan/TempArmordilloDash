using Godot;
using System;
using System.Diagnostics;
using System.Net;

public partial class Player : CharacterBody2D
{
    [Export] public float mass = 1;
    [Export] public float speed = 200f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 3000;
    [Export] public float friction = 0.05f;
    [Export] public Control arrow;
    private Vector2 direction = new Vector2(1, 0);
    private Vector2 forwardDir = new Vector2(1, 0);
    private bool pressingRight = false;
    private bool pressingLeft = false;
    private bool pressingJump = false;
    private bool bounced = false;
    private Vector2 lastVelocity = Vector2.Zero;
    private float prevRot = 0;
    private AnimationPlayer animationPlayer;
    private bool isOnSlope = false;
    private Sprite2D sprite;

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
        sprite = GetNode<Sprite2D>("Sprite");
        animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
        animationPlayer.Play("Idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        float floorAngle = 0;
        bool bounced = false;
        if (lastVelocity != Vector2.Zero)
        {
            isOnSlope = false;
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                //GD.Print("Colliding with " + ((Node)GetSlideCollision(i).GetCollider()).GetParent<ColorRect>().Name);
                Vector2 collisionNormal = GetSlideCollision(i).GetNormal();
                //GD.Print("Rotation of " + lastVelocity + " and " + collisionNormal + " = " + RotationOfVectors(lastVelocity, collisionNormal));
                float collisionAngle = RotationOfVectors(direction, collisionNormal);
                if (collisionAngle > 160f)
                {
                    //Velocity = lastVelocity.Bounce(collisionNormal);
                    //GD.Print("Bounce Velocity: " + Velocity);
                    bounced = true;
                    direction.X *= -1;
                }
                else
                {
                    Velocity = lastVelocity.Slide(collisionNormal);
                }

                if (collisionAngle is < 150 and > 120)
                {
                    isOnSlope = true;
                    floorAngle = RotationOfVectors(collisionNormal, Vector2.Up);
                }
            }
        }
        
        if(!IsOnFloor())
            Velocity += new Vector2(0, 983.4f * mass * (float)delta);

        if (Velocity.X is < 0.5f and > -0.5f)
            Velocity = new Vector2(0, Velocity.Y);

        float forwardForce = 0;

        //Input
        if (pressingJump && IsOnFloor())
            Velocity += new Vector2(0, -jumpForce * 10 * (float)delta);

        /*if (!pressingJump && !IsOnFloor() && Velocity.Y < 0)
            Velocity = new Vector2(Velocity.X, 0);*/
        
        Vector2 addedForce = Vector2.Zero;
        
        if (pressingRight)
        {
            forwardForce = speed * 10 * (float)delta * Mathf.Min(maxSpeed - Velocity.X, maxSpeed) / maxSpeed;
            addedForce = forwardForce * Vector2.Right;
            addedForce.Rotated(floorAngle);
            //GD.Print(addedForce);
        }
        
        if (pressingLeft)
        {
            forwardForce = speed * 10 * (float)delta * Mathf.Abs(Mathf.Max(maxSpeed - Velocity.X, -maxSpeed)) / maxSpeed;
            addedForce = forwardForce * Vector2.Left;
            addedForce.Rotated(floorAngle);
            //GD.Print(addedForce);
        }
        
        Velocity += addedForce;

        if (!pressingRight && IsOnFloor())
            Velocity -= new Vector2(friction * Velocity.X, 0);
        
        UpdateSprite();
        
        lastVelocity = Velocity;


        MoveAndSlide();
    }

    private Vector2 GetSlopePotency(Vector2 angleDirection)
    {
        float rot = RotationOfVectors(angleDirection, direction);
        float potency = 1;
        if(rot is < 90 and > 0)
            potency = rot / 90f;
        GD.Print(angleDirection);
        return angleDirection * potency;
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

        if (@event.IsActionPressed("jump"))
            pressingJump = true;
        else if (@event.IsActionReleased("jump"))
            pressingJump = false;

        if (@event.IsActionPressed("left"))
            pressingLeft = true;
        else if(@event.IsActionReleased("left"))
            pressingLeft = false;
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
        if(Velocity.X > 0 && sprite.Scale.X < 0)
            sprite.Scale = new Vector2(1, 1);
        else if(Velocity.X < 0 && sprite.Scale.X > 0)
            sprite.Scale = new Vector2(-1, 1);
        
        if (Mathf.Abs(Velocity.X) > 0)
        {
            if(Mathf.Abs(lastVelocity.X) <= 0)
                animationPlayer.Play("Roll");

            if (Mathf.Abs(Velocity.X) >= 200 && Mathf.Abs(lastVelocity.X) < 200)
                GetNode<Sprite2D>("Sprite").Frame = 4;
            
            if (Mathf.Abs(Velocity.X) < 200 && Mathf.Abs(lastVelocity.X) >= 200)
                GetNode<Sprite2D>("Sprite").Frame = 0;
            
            animationPlayer.SpeedScale = Velocity.X / maxSpeed * 5;
            
            GD.Print(animationPlayer.SpeedScale);
        }
        else if(Mathf.Abs(lastVelocity.X) > 0)
        {
            animationPlayer.Play("Idle");
            animationPlayer.SpeedScale = 1;
        }
    }

    private float ValueFurtherFromZero(float val1, float val2)
    {
        if(Mathf.Abs(val1) > Mathf.Abs(val2))
            return val1;
        return val2;
    }
}