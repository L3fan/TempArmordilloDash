using Godot;
using System;

public partial class Obstacle: Node2D
{
	private float prevRot;
	private Vector2 prevPos;
	[Export] public ObstacleRarity Rarity { get; private set; }
	[Export] public StaticBody2D collisionBody;
	[Export] public CollisionPolygon2D collisionPolygon;
	[Export] public CollisionShape2D collisionShape;
	[Export] private Node2D usedCollisionType;
	[Export] public Vector2 offsetPosition;
	
	private bool setupComplete = false;


	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if (usedCollisionType == null || !setupComplete)
			return;
		
		collisionBody.ConstantLinearVelocity = (usedCollisionType.GlobalPosition - prevPos)/ (float)delta * 0.9f;
		collisionBody.ConstantAngularVelocity = (usedCollisionType.GlobalRotation - prevRot) / (float)delta;
		
		prevPos = usedCollisionType.GlobalPosition;
		prevRot = usedCollisionType.GlobalRotation;
	}
	
	public virtual void Setup(ObstacleSpot spot)
	{
		Position = spot.Position + offsetPosition;
		Rotation = spot.Rotation;
		Scale = spot.Scale;
		
		
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

		setupComplete = true;
	}
}

public enum ObstacleRarity
{
	COMMON,
	UNCOMMON,
	RARE
}
