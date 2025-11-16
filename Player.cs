using Godot;
using System;
using System.Diagnostics;
using System.Net;
using System.Numerics;
using Vector2 = Godot.Vector2;

public partial class Player : CharacterBody2D
{
    [Export] public float mass = 1;
    [Export] public float speed = 200f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 3000;
    [Export] public float friction = 0.05f;
    [Export] public float bounciness = 1.0f;
    [Export] public Control arrow;
    private Vector2 direction = new Vector2(1, 0);
    private Vector2 forwardDir = new Vector2(1, 0);
    private bool pressingRight = false;
    private bool pressingLeft = false;
    private bool pressingJump = false;
    private Vector2 lastVelocity = Vector2.Zero;
    private float prevRot = 0;
    private AnimationPlayer animationPlayer;
    private Sprite2D sprite;
    private String velocityInfo;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Sprite");
        animationPlayer = GetNode<AnimationPlayer>("Sprite/AnimationPlayer");
        animationPlayer.Play("Idle");
        GD.Print(Mathf.Max(-2000, -5000));
        
        GD.Print("Player is ready");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        velocityInfo = "";
        float floorAngle = 0;
        if (lastVelocity != Vector2.Zero)
        {
            String collisions = "";
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                Vector2 collisionNormal = GetSlideCollision(i).GetNormal();
                float collisionAngle = RotationOfVectors(direction, collisionNormal);
                if (collisionAngle < 15f)
                {
                    Velocity = lastVelocity.Bounce(collisionNormal) * bounciness;
                    velocityInfo += "Post Bounce Calc (" + ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name + "): " + Velocity +"\n";
                }
                else
                {
                    Velocity = lastVelocity.Slide(collisionNormal);
                    velocityInfo += "Post Slide Calc (" + ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name + "): " + Velocity + " " + collisionNormal +"\n";
                }
                
                float floorAngle2 = GetSlideCollision(i).GetAngle() * 180 / Mathf.Pi;
                collisions += ((Node)GetSlideCollision(i).GetCollider()).GetParent().Name + ": " + floorAngle2 + " // " + collisionNormal + "\n";
                if(floorAngle2 < 15)
                {
                    Velocity *= Vector2.Right;
                }
            }
        }
        
        Velocity += new Vector2(0, 983.4f * mass * (float)delta);
        velocityInfo += "Post Gravity: " + Velocity + "\n";

        if (Velocity.X is < 0.5f and > -0.5f)
            Velocity = new Vector2(0, Velocity.Y);

        float forwardForce = 0;

        //Input
        if (pressingJump && IsOnFloor())
        {
            Velocity *= Vector2.Right;
            Velocity += new Vector2(0, -jumpForce * 10 * (float)delta);
        }

        velocityInfo += "Post Jump: " + Velocity + "\n";
        
        Vector2 addedForce = Vector2.Zero;
        
        if (pressingRight)
        {
            forwardForce = speed * 10 * (float)delta * Mathf.Min(maxSpeed - Velocity.X, maxSpeed) / maxSpeed;
            addedForce = forwardForce * Vector2.Right;
        }
        
        if (pressingLeft)
        {
            forwardForce = speed * 10 * (float)delta * Mathf.Abs(Mathf.Max(-maxSpeed - Velocity.X, -maxSpeed)) / maxSpeed;
            addedForce = forwardForce * Vector2.Left;
        }
        
        Velocity += addedForce;
        velocityInfo += "Post Force Add: " + Velocity + "\n";

        if (!(pressingRight &&  Velocity.X > 0 || pressingLeft &&  Velocity.X < 0) && IsOnFloor())
            Velocity -= new Vector2(friction * Velocity.X, 0);
        
        velocityInfo += "Post Jump: " + Velocity + "\n";
        
        //GD.Print(velocityInfo);
        
        UpdateSprite();
        
        lastVelocity = Velocity;

        MoveAndSlide();
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
            sprite.Scale = new Vector2(Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);
        else if(Velocity.X < 0 && sprite.Scale.X > 0)
            sprite.Scale = new Vector2(-Mathf.Abs(sprite.Scale.X), sprite.Scale.Y);
        
        if (Mathf.Abs(Velocity.X) > 0)
        {
            if(Mathf.Abs(lastVelocity.X) <= 0)
                animationPlayer.Play("Roll");

            switch (Mathf.Abs(Velocity.X))
            {
                case float n when (n < 800):
                    GetNode<Sprite2D>("Sprite").Frame = 0;
                    break;
                
                case float n when (n >= 800 && n < 1600):
                    GetNode<Sprite2D>("Sprite").Frame = 3;
                    break;
                
                case float n when (n >= 1600 && n < 3200):
                    GetNode<Sprite2D>("Sprite").Frame = 4;
                    break;
                
                case float n when (n >= 3200):
                    GetNode<Sprite2D>("Sprite").Frame = 5;
                    break;
            }

            animationPlayer.SpeedScale = Velocity.X / maxSpeed * 5;
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