using Godot;
using System;
using System.Threading;
using System.Threading.Tasks;

public partial class DirectionChanger : Area2D
{
	[Export] Vector2 direction = Vector2.Right;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public async void _on_body_entered(Node2D body)
	{
		//GD.Print("Body " + body.Name + " entered");
		if (body.IsInGroup("Player"))
		{
			((Player)body).SetDirection(direction);
		}
	}
}
