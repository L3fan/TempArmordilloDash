using Godot;
using System;
using Godot.Collections;

public partial class AudioHandler : Node2D
{
	public AudioStreamPlayer selectSFX;
	public AudioStreamPlayer cancelSFX;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		selectSFX = GetNode<AudioStreamPlayer>("SelectSFXPlayer");
		cancelSFX = GetNode<AudioStreamPlayer>("CancelSFXPlayer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play(MainMenuSFXType sfxType)
	{
		switch (sfxType)
		{
			case MainMenuSFXType.SELECT:
				selectSFX.Play();
				break;
			case MainMenuSFXType.CANCEL:
				cancelSFX.Play();
				break;
		}
	}
}

public enum MainMenuSFXType
{
	SELECT,
	CANCEL
}
