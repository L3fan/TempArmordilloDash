using Godot;
using System;

public partial class DebugScreen : Node
{
	[Export] public TextEdit velocityDisplay;
	[Export] public Player player;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		velocityDisplay.Text = "Velocity: \nX: " + player.Velocity.X + "\nY: " + player.Velocity.Y;
	}
}
