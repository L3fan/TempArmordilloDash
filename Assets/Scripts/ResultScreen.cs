using Godot;
using System;
using System.IO;

public partial class ResultScreen : Node2D
{
	[Export] public Control leaderboard;
	[Export] public RichTextLabel namesLabel;
	[Export] public RichTextLabel timesLabel;

	[Export] public LineEdit nameEntry;
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
		if (nameEntry.Text.Length == 0)
		{
			nameEntry.PlaceholderText = "Must enter Name";
			return;
		}

		if (nameEntry.Text.Length < 4)
		{
			nameEntry.Text = "";
			nameEntry.PlaceholderText = "Must be 4 letters";
			return;
		}

		GetChild<Control>(0).Visible = false;
		leaderboard.Visible = true;
		GD.Print(Save.Instance != null);
		int newPosition = Save.Instance.PlaceEntryOnLeaderboard(nameEntry.Text, GameManager.Instance.totalTime);
		Save.Instance.ShowLeaderboard(namesLabel, timesLabel, newPosition);
		Save.Instance.SaveLeaderboard();
	}

	public void _OnFinishButtonPressed()
	{
		Save.Instance.SaveLeaderboard();
		GameManager.Instance.Load(SceneType.MainMenu);
	}

	public void _OnLineEditTextChanged(string newText)
	{
		nameEntry.SetText(newText.ToUpper());
		nameEntry.CaretColumn = nameEntry.Text.Length;
	}
}
