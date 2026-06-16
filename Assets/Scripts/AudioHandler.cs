using System.Collections.Generic;
using Godot;
using FmodSharp;

public partial class AudioHandler : Node2D
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

	public void Play(MainMenuSFXType type)
	{
		FmodEvent sfx = null;
		switch (type)
		{
			case MainMenuSFXType.Confirm:
				soundeffectEvents.TryGetValue("Confirm", out sfx);
				break;
			case MainMenuSFXType.Cancel:
				soundeffectEvents.TryGetValue("Cancel", out sfx);
				break;
		}
		sfx?.Start();
	}
}

public enum MainMenuSFXType
{
	Confirm,
	Cancel
}
