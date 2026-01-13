using Godot;
using System;

public partial class Goal : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void _OnArea2dBodyEntered(Node2D body)
	{
		if (body is Player)
		{
			FinishLevel();
		}
	}

	private void FinishLevel()
	{
		GD.Print("Finished Level!");
	}
}
