using Godot;
using System;

public partial class Stalagtite : Obstacle
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void Setup(ObstacleSpot spot)
	{
		base.Setup(spot);
		Position = spot.Position + spot.ceilingSpot.Position + offsetPosition;
	}
}
