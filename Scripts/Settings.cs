using Godot;
using System;
using System.Collections.Generic;

public partial class Settings : Node2D
{
	public static Settings Instance { get; private set; }
	public String mainMenuScenePath;
	public String playerScenePath;
	public String trailScenePath;
	public String levelUIScenePath;
	public String obstaclesFolderPath;

	private List<PackedScene> obstacles;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
		mainMenuScenePath = "res://Scenes/MainMenuScene.tscn";
		
		playerScenePath = "res://Scenes/Player.tscn";
		
		trailScenePath = "res://Scenes/DashTrail.tscn";

		levelUIScenePath = "res://Scenes/LevelUI.tscn";
		
		obstaclesFolderPath = "res://Scenes/Obstacles/";

		LoadObstacles();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void LoadObstacles()
	{
		obstacles = new List<PackedScene>();
		var dir = DirAccess.Open(obstaclesFolderPath);
		dir.ListDirBegin();
		String fileName = dir.GetNext();
		while (fileName != String.Empty)
		{
			if (fileName.EndsWith(".tscn"))
				obstacles.Add(GD.Load<PackedScene>(obstaclesFolderPath + fileName));
			fileName = dir.GetNext();
		}
	}

	public Node2D GetRandomObstacle()
	{
		return (Node2D)obstacles[new Random().Next(0, obstacles.Count)].Instantiate();
	}
}
