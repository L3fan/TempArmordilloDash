using System.Collections.Generic;
using Godot;
using FmodSharp;

public partial class AudioHandler : Node
{
	[Export] public Godot.Collections.Dictionary<string, string> soundeffectPaths;
	[Export] public Godot.Collections.Dictionary<string, FmodEvent> soundeffectEvents;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (KeyValuePair<string, string> entry in soundeffectPaths)
		{
			FmodEvent sfxEvent = FmodServerWrapper.CreateEventInstance(entry.Value);
			if(!soundeffectEvents.ContainsKey(entry.Key))
				soundeffectEvents.Add(entry.Key, sfxEvent);
			else
			{
				soundeffectEvents[entry.Key] = sfxEvent;
			}
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play(string sfxName)
	{
		FmodEvent sfx;
		soundeffectEvents.TryGetValue(sfxName, out sfx);
		sfx?.Start();
	}

	public void PlaySingle(string sfxName)
	{
		string sfxPath;
		soundeffectPaths.TryGetValue(sfxName, out sfxPath);
		if (sfxPath == null)
			return;
		FmodEvent sfx = FmodServerWrapper.CreateEventInstance(sfxPath);
		sfx.ProcessMode = ProcessModeEnum.Pausable;
		sfx?.Start();

	}
	
	
	public void PlayContinuous(string sfxName)
	{
		FmodEvent sfx;
		soundeffectEvents.TryGetValue(sfxName, out sfx);
		sfx?.Start();
	}

	public void Stop(string sfxName)
	{
		FmodEvent sfx;
		soundeffectEvents.TryGetValue(sfxName, out sfx);
		sfx?.Stop();
	}
}

public enum MainMenuSFXType
{
	Confirm,
	Cancel
}
