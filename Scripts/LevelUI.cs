using Godot;
using System;

public partial class LevelUI : Node2D
{
	[Export] public CameraMovement camera;
	[Export] public DebugScreen debugScreen;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Setup(Player player)
	{
		camera.player = player;
		debugScreen.player = player;
		
		PackedScene trailScene = GD.Load<PackedScene>(Settings.Instance.trailScenePath);
		var dashTrail = trailScene.Instantiate();
		dashTrail.ProcessMode = ProcessModeEnum.Pausable;
		AddChild(dashTrail);
		((DashTrail)dashTrail).player = player;
	}
}
