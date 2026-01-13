using Godot;
using System;

public partial class CeilingSpinner : Spinner
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public override void Setup(ObstacleSpot spot)
	{
		base.Setup(spot);
		Position += spot.ceilingSpot.Position;
	}
}
