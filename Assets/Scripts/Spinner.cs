using Godot;
using System;

public partial class Spinner : Obstacle
{
	[Export] public float rotationSpeed;

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		RotationDegrees += rotationSpeed * (float)delta;
	}
}
