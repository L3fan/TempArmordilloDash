using Godot;
using System;
using System.Diagnostics;
using System.Net;

public partial class Player : CharacterBody2D
{
    [Export] public float mass = 1;
    [Export] public float speed = 100f;
    [Export] public float maxSpeed = 3000;
    [Export] public float jumpForce = 3000;
    [Export] public float friction = 0.05f;
    [Export] public Control arrow;
    private Vector2 direction = new Vector2(1, 0);
    private Vector2 forwardDir = new Vector2(1, 0);
    private bool pressingForward = false;
    private bool pressingJump = false;
    private bool bounced = false;
    private Vector2 lastVelocity = Vector2.Zero;
    private float prevRot = 0;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        GD.Print("Player is ready");
        Vector2 vecA = new Vector2(0, 10);
        Vector2 vecB = new Vector2(0, -1);
        Vector2 projNV = vecA.Normalized() * vecB / Mathf.Pow(vecB.Length(), 2) * vecB;
        Vector2 c = vecA.Normalized() - projNV;
        GD.Print(vecA.Slide(projNV + c));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        if (lastVelocity != Vector2.Zero)
        {
            for (int i = 0; i < GetSlideCollisionCount(); i++)
            {
                Vector2 collisionNormal = GetSlideCollision(i).GetNormal();
                //GD.Print("Rotation of " + lastVelocity + " and " + collisionNormal + " = " + RotationOfVectors(lastVelocity, collisionNormal));
                if (RotationOfVectors(direction, collisionNormal) > 130f)
                {
                    Velocity = lastVelocity.Bounce(collisionNormal);
                    //GD.Print(Velocity);
                    direction.X *= -1;
                }
            }
        }
        if(!IsOnFloor())
            Velocity += new Vector2(0, 983.4f * mass * (float)delta);

        if (Velocity.X < 0.5f && Velocity.X > -0.5f)
            Velocity = new Vector2(0, Velocity.Y);

        float forwardForce = speed * 10 * (float)delta * (maxSpeed - Mathf.Abs(Velocity.X)) / maxSpeed;
        //GD.Print(forwardForce);

        //Input
        if (pressingForward)
            Velocity += forwardForce * direction;

        if (!pressingForward)
            Velocity -= new Vector2(friction * Velocity.X, 0);

        if (pressingJump && IsOnFloor())
            Velocity += new Vector2(0, -jumpForce * 10 * (float)delta);

        if (!pressingJump && !IsOnFloor() && Velocity.Y < 0)
            Velocity = new Vector2(Velocity.X, 0);
        
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
}