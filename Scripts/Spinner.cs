using Godot;
using System;

public partial class Spinner : Node2D, Obstacle
{
	[Export] public float rotationSpeed;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Rotation += rotationSpeed * (float)delta;
	}

	public void Setup(Node2D ceilingSpot)
	{
		
	}
}
