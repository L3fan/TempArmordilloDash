using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export] public Player player;

	[Export] public Vector2 bufferZone;

	private ColorRect slowDownTint;

	private ColorRect blackScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		slowDownTint = GetNode<ColorRect>("SlowDownTint");
		blackScreen = GetNode<ColorRect>("BlackScreen");
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

	public void DarkenScreen(float amount)
	{
		blackScreen.Color = new Color(blackScreen.Color.R, blackScreen.Color.G, blackScreen.Color.B, blackScreen.Color.A + amount);
	}
	
	public bool NoBlackScreen()
	{
		return blackScreen.Color.A <= 0.0f;
	}

	public bool ScreenIsBlack()
	{
		return blackScreen.Color.A >= 1.0f;
	}
}
