using Godot;
using System;

public partial class Tripwire : Node2D
{
	[Export] public HammerTrap hammerTrap;
	[Export] public ColorRect wire;

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
		if (activated || body is not Player)
			return;
		wire.Visible = false;
		activated = true;
		hammerTrap.Activate();
	}
}
