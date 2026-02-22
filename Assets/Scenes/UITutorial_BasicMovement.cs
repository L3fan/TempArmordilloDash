using Godot;
using System;

public partial class UITutorial_BasicMovement : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node child in GetChildren())
		{
			if (child is not Sprite2D)
				continue;
			
			AnimationPlayer animationPlayer = child.GetChild<AnimationPlayer>(0);
			animationPlayer.Play();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
