using Godot;
using System;

public partial class Tutorial : Panel
{
	[Export] public PauseMenu pauseMenu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().Paused = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnContinueButtonPressed()
	{
		pauseMenu.Enable();
		GetTree().Paused = false;
		Visible = false;
	}
}
