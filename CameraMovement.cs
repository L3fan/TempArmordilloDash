using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export] public Player player;

	[Export] public Vector2 bufferZone;

	private ColorRect slowDownTint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		slowDownTint = GetNode<ColorRect>("SlowDownTint");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//Camera Movement
		if (player.Position.X - Position.X > bufferZone.X)
			Position = new Vector2(player.Position.X - bufferZone.X, Position.Y);
		
		if (player.Position.X - Position.X < -bufferZone.X)
			Position = new Vector2(player.Position.X + bufferZone.X, Position.Y);
		
		if (player.Position.Y - Position.Y > bufferZone.Y)
			Position = new Vector2(Position.X, player.Position.Y - bufferZone.Y);
		
		if (player.Position.Y - Position.Y < -bufferZone.Y)
			Position = new Vector2(Position.X, player.Position.Y + bufferZone.Y);
			
		//Set Slowdown Tint
		if(player.IsInSlowDown())
			slowDownTint.Color = new Color(slowDownTint.Color.R, slowDownTint.Color.G, slowDownTint.Color.B, 0.25f);
		else
			slowDownTint.Color = new Color(slowDownTint.Color.R, slowDownTint.Color.G, slowDownTint.Color.B, 0);
	}
}
