using Godot;
using System;

public partial class Spinner : Obstacle
{
	public AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetChild(0).GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.CurrentAnimation = "Spin";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
