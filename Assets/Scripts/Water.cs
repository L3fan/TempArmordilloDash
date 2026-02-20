using Godot;
using System;

public partial class Water : TextureRect
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnBodyEnteredWater(Node2D body)
	{
		if (body is not Player)
			return;

		Player player = body as Player;

		player.SetEnvState(EnvState.WATER);
	}
	
	public void _OnBodyExitedWater(Node2D body)
	{
		if (body is not Player)
			return;

		Player player = body as Player;

		player.SetEnvState(EnvState.DEFAULT);
	}
}
