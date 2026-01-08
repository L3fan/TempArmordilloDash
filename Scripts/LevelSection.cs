using Godot;
using System;

public partial class LevelSection : Node2D
{
	[Export] public Node2D startPointNode;
	[Export] public Node2D endPointNode;
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
		
		GameManager.Instance.GameOver(true);
	}
}
