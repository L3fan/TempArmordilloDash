using Godot;
using System;

public partial class Level : Node
{
	[Export] public LevelType levelType;
	[Export] public Node2D spawnPoint;
	public LevelUI levelUI;
	public Vector2 spawnPointPos = Vector2.Zero;
	
	public override void _Ready()
	{
		spawnPointPos = spawnPoint.GlobalPosition;
		
		//Create Player
		PackedScene playerScene = GD.Load<PackedScene>(Settings.Instance.playerScenePath);
		var playerNode = playerScene.Instantiate();
		Player player = playerNode as Player;
		player.Position = spawnPointPos;
		player.LinearVelocity = spawnPoint.GetChild<Node2D>(4).Position;
		AddChild(player);
		GameManager.Instance.player = player;

		Setup();
	}

	public void Setup()
	{
		//Create the necessary UI for a level
		PackedScene levelUIScene = GD.Load<PackedScene>(Settings.Instance.levelUIScenePath);
		levelUI = (LevelUI)levelUIScene.Instantiate();
		levelUI.Setup(GameManager.Instance.player);
		AddChild(levelUI);
	}
}

public enum LevelType
{
	Default,
	Debug
}
