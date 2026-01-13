using Godot;
using System;
using System.Collections.Generic;

public partial class Settings : Node2D
{
	public static Settings Instance { get; private set; }
	[Export] public String mainMenuScenePath;
	[Export] public String playerScenePath;
	[Export] public String trailScenePath;
	[Export] public String levelUIScenePath;
	[Export] public String obstaclesFolderPath;
	public String levelScenesFolderPath;
	public String levelSectionsFolderPath;
	public String levelEndSectionPath;

	private List<PackedScene> obstacles;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		String sceneFolder = "res://Assets/Scenes/";
		
		mainMenuScenePath = sceneFolder + "MainMenuScene.tscn";
		
		playerScenePath = sceneFolder + "Player.tscn";
		
		trailScenePath = sceneFolder + "DashTrail.tscn";

		levelUIScenePath = sceneFolder + "LevelUI.tscn";
		
		obstaclesFolderPath = sceneFolder + "Obstacles/";
		
		levelScenesFolderPath = sceneFolder + "Levels/";
		
		levelSectionsFolderPath = levelScenesFolderPath + "LevelSections/";
		
		levelEndSectionPath = sceneFolder + "EndLevelSection.tscn";

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

	public Obstacle GetRandomObstacle()
	{
		return (Obstacle)obstacles[new Random().Next(0, obstacles.Count)].Instantiate();
	}
}
