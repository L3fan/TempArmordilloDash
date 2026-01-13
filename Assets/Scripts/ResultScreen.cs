using Godot;
using System;
using System.IO;

public partial class ResultScreen : Node2D
{
	[Export] public Node2D timeResult;
	[Export] public Control leaderboard;
	[Export] public RichTextLabel timesLabel;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnContinueButtonPressed()
	{
		timeResult.Visible = false;
		leaderboard.Visible = true;
		GD.Print(Save.Instance != null);
		Save.Instance.LoadLeaderboard(timesLabel);
	}

	public void _OnFinishButtonPressed()
	{
		Save.Instance.SaveLeaderboard();
		GameManager.Instance.Load(SceneType.MainMenu);
	}
}
