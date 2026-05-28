using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export] public Player player;

	[Export] public Vector2 bufferZone;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player.Position.X - Position.X > bufferZone.X)
			Position = new Vector2(player.Position.X - bufferZone.X, Position.Y);
		
		if (player.Position.X - Position.X < -bufferZone.X)
			Position = new Vector2(player.Position.X + bufferZone.X, Position.Y);
			
	}
}
