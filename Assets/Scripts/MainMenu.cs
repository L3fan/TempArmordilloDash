using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public Control doubleCheck;

	[Export] public Control leaderboardDisplay;
	[Export] public RichTextLabel nameEntry;
	[Export] public RichTextLabel timeEntry;

	[Export] public Control settingsDisplay;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_OnMasterVolumeUpdate(0.7f);
		_OnMusicVolumeUpdate(0.7f);
		_OnSFXVolumeUpdate(0.7f);
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

	public void _OnLeaderboardButtonPressed()
	{
		leaderboardDisplay.Visible = true;
		Save.Instance.ShowLeaderboard(nameEntry, timeEntry);
	}
	
	public void _OnCloseLeaderboardButtonPressed()
	{
		leaderboardDisplay.Visible = false;
		nameEntry.Text = "Name";
		timeEntry.Text = "Time";
	}

	public void _OnMasterVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Master");
	}

	public void _OnMusicVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Music");
	}

	public void _OnSFXVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Soundtrack");
	}

	public void _OnSettingsButtonPressed()
	{
		settingsDisplay.Visible = true;
	}

	public void _OnCloseSettingsButtonPressed()
	{
		settingsDisplay.Visible = false;
	}
}
