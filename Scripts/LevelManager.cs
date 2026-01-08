using Godot;
using System;
using System.Collections.Generic;

public partial class LevelManager : Node2D
{
	[Export] public String[] levelScenesPaths;
	private List<PackedScene> levelScenes = new List<PackedScene>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		foreach (String levelScenePath in levelScenesPaths)
		{
			var scene = GD.Load<PackedScene>(levelScenePath);
			levelScenes.Add(scene);
		}
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
