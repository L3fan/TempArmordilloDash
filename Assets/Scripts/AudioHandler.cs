using System.Collections.Generic;
using Godot;
using FmodSharp;

public partial class AudioHandler : Node
{
	[Export] public Godot.Collections.Dictionary<string, FmodEvent> events;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (var child in GetChildren())
		{
			if (child is not FmodEvent)
				return;
			FmodEvent sfxEvent = (FmodEvent)child;
			events.Add(child.Name, sfxEvent);
			GD.Print("Added " + child.Name);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play(string sfxName)
	{
		FmodEvent sfx;
		events.TryGetValue(sfxName, out sfx);
		sfx?.Start();
	}

	public void Stop(string sfxName)
	{
		FmodEvent sfx;
		events.TryGetValue(sfxName, out sfx);
		sfx?.Stop();
	}
}

public enum MainMenuSFXType
{
	Confirm,
	Cancel
}
