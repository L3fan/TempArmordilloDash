using Godot;
using System;
using Godot.Collections;

public partial class AudioHandler : Node2D
{
	[Export] public Dictionary<string, string> soundeffects;

	[Export] private AudioStreamPlayer chosenSFX;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print(soundeffects);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play(string soundeffectName)
	{
		bool succeeded = soundeffects.TryGetValue(soundeffectName, out string sfxPath);
		
		if (!succeeded)
			return;
		
		chosenSFX = GetNode<AudioStreamPlayer>(sfxPath);
		
		chosenSFX.Play();
	}
}
