using Godot;
using System;

public partial class Tripwire : Node2D, Obstacle
{
	[Export] public HammerTrap hammer;

	private bool activated = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void _OnBodyEnteredWire(Node2D body)
	{
		if (activated)
			return;
		Visible = false;
		activated = true;
		hammer.Activate();
	}

	public void Setup(Node2D ceilingSpot)
	{
		hammer.Position = ceilingSpot.Position;
		hammer.Rotation = ceilingSpot.Rotation;
		hammer.Setup();
		
	}
}
