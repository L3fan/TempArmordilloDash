using Godot;
using System;

public partial class HammerTrap : Obstacle
{
	[Export] public Node2D hammer;
	private bool activated = false;
	private bool finished = false;

	private float startRotation;
	private float endRotation;

	private float speed = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		if (activated && !finished)
		{
			hammer.RotationDegrees += (float)delta * speed;
			speed = Mathf.Min(300, speed * 2f);
			

			if (hammer.RotationDegrees > endRotation)
			{
				hammer.RotationDegrees = endRotation;
				finished = true;
			}
		}
	}

	public void Activate()
	{
		activated = true;
	}

	public override void Setup(ObstacleSpot spot)
	{
		base.Setup(spot);
		hammer.Position = spot.ceilingSpot.Position;
		hammer.Rotation = spot.ceilingSpot.Rotation;
		startRotation = hammer.RotationDegrees;
		endRotation = hammer.RotationDegrees + 180;
	}
}
