using Godot;
using System;

public partial class Settings : Node2D
{
	public static Settings Instance { get; private set; }
	public String mainMenuScenePath;
	public String playerScenePath;
	public String trailScenePath;
	public String levelUIScenePath;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		
		mainMenuScenePath = "res://Scenes/MainMenuScene.tscn";
		
		playerScenePath = "res://Scenes/Player.tscn";
		
		trailScenePath = "res://Scenes/DashTrail.tscn";

		levelUIScenePath = "res://Scenes/LevelUI.tscn";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
