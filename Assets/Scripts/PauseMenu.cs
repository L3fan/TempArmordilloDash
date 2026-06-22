using Godot;
using System;
using System.Diagnostics;
using FmodSharp;

public partial class PauseMenu : ColorRect
{
	private bool pauseMenuOpen = false;

	private bool enabled = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _Input(InputEvent @event)
	{
		if (!enabled)
			return;
		
		if (@event.IsActionPressed("Pause"))
		{
			SetPauseMenu(!pauseMenuOpen);
		}
	}

	private void SetPauseMenu(bool setPauseMenu)
	{
		GetTree().Paused = setPauseMenu;
		Visible = setPauseMenu;
		pauseMenuOpen = setPauseMenu;
		
	}

	private void _OnResumeButtonPressed()
	{
		SetPauseMenu(false);
	}

	private void _OnExitLevelButtonPressed()
	{
		GameManager.Instance.Load(SceneType.MainMenu);
	}

	public void Enable()
	{
		enabled = true;
	}
}
