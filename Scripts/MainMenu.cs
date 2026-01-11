using Godot;
using System;

public partial class MainMenu : Node2D
{
	[Export] public Control doubleCheck;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnStartButtonPressed()
	{
		GameManager.Instance.Load(SceneType.Level);
	}

	public void _OnEndGameButtonPressed()
	{
		GetTree().Quit();
	}

	public void _OnResetButtonPressed()
	{
		doubleCheck.Visible = true;
	}

	public void _OnConfirmResetButtonPressed()
	{
		Save.Instance.DeleteLeaderboard();
		doubleCheck.Visible = false;
	}

	public void _OnCancelResetButtonPressed()
	{
		doubleCheck.Visible = false;
	}
}
