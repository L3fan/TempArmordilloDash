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
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (obstacleSpots != null)
		{
			Random random = new Random();
			int amountOfObstacles = 0;

			bool hasObstacles = random.Next(2) != 0;
			if(true)
				amountOfObstacles = random.Next(1, obstacleSpots.Length);
			List<ObstacleSpot> availableSpots = obstacleSpots.ToList();
			List<ObstacleSpot> chosenSpots = new List<ObstacleSpot>();
			for (int i = 0; i < amountOfObstacles; i++)
			{
				ObstacleSpot chosenSpot = availableSpots[random.Next(0, availableSpots.Count)];
				chosenSpots.Add(chosenSpot);
				availableSpots.Remove(chosenSpot);
			}

			String obstacleTypes = "";
			foreach(var spot in chosenSpots)
			{
				Obstacle randObstacle = Settings.Instance.GetRandomObstacle();
				randObstacle.Setup(spot);
				AddChild(randObstacle);
				obstacleTypes += randObstacle.Name + ", ";
			}
			String debugMesssage = "Created " + amountOfObstacles + "obstacle(s): " + obstacleTypes;
			//GD.Print(debugMesssage);
		}
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
}

public enum SectionRarity
{
	COMMON,
	UNCOMMON,
	RARE
}