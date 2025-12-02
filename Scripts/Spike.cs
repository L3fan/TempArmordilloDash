using Godot;
using System;
using System.Diagnostics;

public partial class Spike : Node2D
{
	[Export] private Area2D damageArea;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnDamageAreaBodyEntered(Node2D collider)
	{
		if (collider is not Player)
			return;
		
		Player player = collider as Player;
		if (player.GetLastVelocity().Y > 0)
		{
			//GD.Print("Took damage (" + player.GetLastVelocity().Y + ")");
			player.TakeDamage(this);
		}
	}
}
