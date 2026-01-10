using Godot;
using System;
using System.Collections.Generic;

public partial class LevelManager : Node2D
{
	[Export] public String levelScenesFolderPath;
	[Export] public String[] levelScenesPaths;
	private List<PackedScene> levelScenes = new List<PackedScene>();

	public static LevelManager Instance { get; private set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		levelScenesFolderPath = "res://Scenes/Levels/";

		GetLevelScenesPaths();

		ProcessMode = ProcessModeEnum.Always;
		foreach (String levelScenePath in levelScenesPaths)
		{
			var scene = GD.Load<PackedScene>(levelScenePath);
			levelScenes.Add(scene);
		}
		
		Instance = this;
	}

	private void GetLevelScenesPaths()
	{
		List<String> tempList = new List<String>();
		var dir = DirAccess.Open(levelScenesFolderPath);
		dir.ListDirBegin();
		String fileName = dir.GetNext();
		while (fileName != String.Empty)
		{
			if (fileName.EndsWith(".tscn"))
				tempList.Add(levelScenesFolderPath + fileName);
			fileName = dir.GetNext();
		}
		levelScenesPaths = tempList.ToArray();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public PackedScene GetLevelScene(LevelType type)
	{
		switch (type)
		{
			case LevelType.Debug:
				return levelScenes[1];
			case LevelType.Default:
			default:
				return levelScenes[0];
		}
	}
}
