using Godot;
using System;

public partial class MainMenu : Control
{
	[Export] public Control doubleCheck;

	[Export] public Control leaderboardDisplay;
	[Export] public RichTextLabel nameEntry;
	[Export] public RichTextLabel timeEntry;

	[Export] public Control settingsDisplay;

	[Export] public AudioHandler audioHandler;

	[Export] private Button quitButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (GameManager.Instance.demo)
			quitButton.Disabled = true;

		GameManager.Instance.onToggleDemo += OnToggleDemo;
		
		Settings.Instance.LoadSettings();
		VolumeSettings vSet = Settings.Instance.settingsSave.VolumeSettings;
		SetVolume(vSet);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnStartButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.SELECT);
		GameManager.Instance.Load(SceneType.Level);
	}

	public void _OnEndGameButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.CANCEL);
		GetTree().Quit();
	}

	public void _OnResetButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.SELECT);
		doubleCheck.Visible = true;
	}

	public void _OnConfirmResetButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.SELECT);
		Save.Instance.DeleteLeaderboard();
		doubleCheck.Visible = false;
	}

	public void _OnCancelResetButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.CANCEL);
		doubleCheck.Visible = false;
	}

	public void _OnLeaderboardButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.SELECT);
		leaderboardDisplay.Visible = true;
		Save.Instance.ShowLeaderboard(nameEntry, timeEntry);
	}
	
	public void _OnCloseLeaderboardButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.CANCEL);
		leaderboardDisplay.Visible = false;
		nameEntry.Text = "Name";
		timeEntry.Text = "Time";
	}

	public void _OnMasterVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Master");
		Settings.Instance.settingsSave.VolumeSettings.MasterVolume = value;
	}

	public void _OnMusicVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Music");
		Settings.Instance.settingsSave.VolumeSettings.MusicVolume = value;
	}

	public void _OnSFXVolumeUpdate(float value)
	{
		GameManager.Instance.UpdateVolume(value, "Soundeffect");
		Settings.Instance.settingsSave.VolumeSettings.SFXVolume = value;
	}

	public void _OnSettingsButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.SELECT);
		settingsDisplay.Visible = true;
	}

	public void _OnCloseSettingsButtonPressed()
	{
		audioHandler.Play(MainMenuSFXType.CANCEL);
		Settings.Instance.SaveSettings();
		settingsDisplay.Visible = false;
	}

	private void SetVolume(VolumeSettings vSet)
	{
		_OnMasterVolumeUpdate(vSet.MasterVolume);
		_OnMusicVolumeUpdate(vSet.MusicVolume);
		_OnSFXVolumeUpdate(vSet.SFXVolume);

		Slider[] volumeSliders = [null, null, null];
		int i = 0;
		foreach (Node child in settingsDisplay.GetChildren())
		{
			if (i > volumeSliders.Length || child is not Label || child.GetChildCount() == 0)
				continue;
			if(child.GetChild(0) is not Slider)
				continue;
			volumeSliders[i] = child.GetChild<Slider>(0);
			i++;
		}

		volumeSliders[0].Value = vSet.MasterVolume;
		volumeSliders[1].Value = vSet.MusicVolume;
		volumeSliders[2].Value = vSet.SFXVolume;
	}

	public void OnToggleDemo()
	{
		quitButton.Disabled = GameManager.Instance.demo;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		GameManager.Instance.onToggleDemo -= OnToggleDemo;
	}
}
