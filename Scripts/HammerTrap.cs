using Godot;
using System;

public partial class HammerTrap : TileMapLayer
{
	private bool activated = false;
	private bool finished = false;

	private float startRotation;
	private float endRotation;

	private float speed = 1f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		if (activated && !finished)
		{
			Rotation = startRotation + (float)delta * speed;
			speed *= 1.25f;

			if (Rotation > endRotation)
			{
				Rotation = endRotation;
				finished = true;
			}
		}
	}

	public void Activate()
	{
		activated = true;
	}

	public void Setup()
	{
		startRotation = Rotation;
		endRotation = Rotation - 180;
	}
}
