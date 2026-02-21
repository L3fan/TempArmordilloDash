using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelSection : Node2D
{
	[Export] public Node2D startPointNode;
	[Export] public Node2D endPointNode;

	[Export] public ObstacleSpot[] obstacleSpots;
	[Export] public SectionRarity rarity = SectionRarity.COMMON;

	private float yDifference = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnGoalBodyEntered(Node2D body)
	{
		if (body is not Player)
			return;
		
		GameManager.Instance.GameOver();
	}

	public float GetYDifference()
	{
		yDifference = startPointNode.GlobalPosition.Y - endPointNode.GlobalPosition.Y;
		return yDifference;
	}
}

public enum SectionRarity
{
	COMMON,
	UNCOMMON,
	RARE
}