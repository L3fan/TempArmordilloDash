using Godot;
using FmodSharp;
using Godot.Collections;

public partial class AudioHandler : Node
{
	[Export] public string[] eventPaths;

	[Export] public Dictionary<string, FmodEvent> events;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (string eventPath in eventPaths)
		{
			FmodEvent sfxEvent = FmodServerWrapper.CreateEventInstance(eventPath);
			sfxEvent.Name = "Event(" + eventPath + ")";
			AddChild(sfxEvent);
			events.Add(eventPath, sfxEvent);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Play(string sfxName)
	{
		events.TryGetValue(sfxName, out FmodEvent sfx);
		if (sfx == null)
		{
			events.Remove(sfxName);
			sfx = FmodServerWrapper.CreateEventInstance(sfxName);
			events.Add(sfxName, sfx);
		}
		sfx?.Start();
	}

	public void Stop(string sfxName)
	{
		events.TryGetValue(sfxName, out FmodEvent sfx);
		sfx?.Stop();
	}

	public void Pause(string sfxName)
	{
		events.TryGetValue(sfxName, out FmodEvent sfx);
		if(sfx != null)
			sfx.Paused = true;
	}

	public FmodEvent GetEvent(string sfxName)
	{
		events.TryGetValue(sfxName, out FmodEvent sfx);
		return sfx;
	}
}

public enum MainMenuSFXType
{
	Confirm,
	Cancel
}
