using Godot;
using System;

public partial class Obstacle: Node2D
{
	private float prevRot;
	private Vector2 prevPos;
	[Export] public StaticBody2D collisionBody;
	[Export] public CollisionPolygon2D collisionPolygon;
	[Export] public CollisionShape2D collisionShape;
	[Export] private Node2D usedCollisionType;
	[Export] public Vector2 offsetPosition;


	public override void _Ready()
	{
		if (collisionPolygon != null)
		{
			usedCollisionType = collisionPolygon;
		} else if (collisionShape != null)
		{
			usedCollisionType = collisionShape;
		}

		if (usedCollisionType != null)
		{
			prevRot = usedCollisionType.GlobalRotation;
			prevPos = usedCollisionType.GlobalPosition;
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		if (usedCollisionType == null)
			return;
		
		collisionBody.ConstantLinearVelocity = (usedCollisionType.GlobalPosition - prevPos)/ (float)delta * 0.9f;
		collisionBody.ConstantAngularVelocity = (usedCollisionType.GlobalRotation - prevRot) / 360f * Mathf.Pi / (float)delta;
		
		prevPos = usedCollisionType.GlobalPosition;
		prevRot = usedCollisionType.GlobalRotation;
	}
	
	public virtual void Setup(ObstacleSpot spot)
	{
		Position = spot.Position + offsetPosition;
		Rotation = spot.Rotation;
		Scale = spot.Scale;
	}
}
