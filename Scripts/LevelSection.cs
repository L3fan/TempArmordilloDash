using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class LevelSection : Node2D
{
	[Export] public Node2D startPointNode;
	[Export] public Node2D endPointNode;

	[Export] public ObstacleSpot[] obstacleSpots;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if (obstacleSpots != null)
		{
			Random random = new Random();
			int amountOfObstacles = 0;
			
			bool hasObstacles = random.Next(2) != 0;
			if(hasObstacles)
				amountOfObstacles = random.Next(1, obstacleSpots.Length);
			List<ObstacleSpot> availableSpots = obstacleSpots.ToList();
			List<ObstacleSpot> chosenSpots = new List<ObstacleSpot>();
			for (int i = 0; i < amountOfObstacles; i++)
			{
				ObstacleSpot chosenSpot = availableSpots[random.Next(0, availableSpots.Count)];
				chosenSpots.Add(chosenSpot);
				availableSpots.Remove(chosenSpot);
			}
			foreach(var spot in chosenSpots)
			{
				Node2D randObstacle = Settings.Instance.GetRandomObstacle();
				randObstacle.Position = spot.Position;
				randObstacle.Rotation = spot.Rotation;
				if (randObstacle is Obstacle)
				{
					Obstacle obstacle = randObstacle as Obstacle;
					obstacle.Setup(spot.ceilingSpot);
				}
				AddChild(randObstacle);
			}
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
