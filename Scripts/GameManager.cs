using Godot;
using System;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	
	[Export] public Settings settings;
	[Export] public LevelManager levelManager;
	[Export] public bool debugLevel = false;
	[Export] public CameraMovement camera;
	[Export] public DebugScreen debugScreen;
	private Level currentLevel = null;
	private PackedScene currentLevelScene = null;

	private Vector2 spawnPoint = Vector2.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Instance = this;
		
		if (debugLevel)
		{
			currentLevelScene = levelManager.GetLevelScene(LevelType.Debug);
		}
		else
		{
			currentLevelScene = levelManager.GetLevelScene(LevelType.Default);
		}

		var instance = currentLevelScene.Instantiate();
		AddChild(instance);

		if (instance is Level)
		{
			Node2D spawnPointNode = ((Level)instance).spawnPoint;
			spawnPoint = spawnPointNode.GlobalPosition;
		}
		
		PackedScene playerScene = GD.Load<PackedScene>("res://Scenes/player.tscn");
		var playerNode = playerScene.Instantiate();
		Player player = playerNode as Player;
		player.Position = spawnPoint;
		AddChild(player);

		if (!debugLevel)
		{
			player.Velocity = new Vector2(0, 1000);
		}
		
		PackedScene trailScene = GD.Load<PackedScene>("res://Scenes/DashTrail.tscn");
		var dashTrail = trailScene.Instantiate();
		dashTrail.ProcessMode = ProcessModeEnum.Pausable;
		AddChild(dashTrail);
		((DashTrail)dashTrail).player = player;
		
		camera.player = player;
		debugScreen.player = player;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void GameOver(bool won)
	{
		GetTree().Paused = true;
		
	}
}
