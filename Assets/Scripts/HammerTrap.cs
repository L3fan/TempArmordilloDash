using Godot;
using System;
using System.Threading.Tasks;

public partial class HammerTrap : Obstacle
{
	[Export] public Node2D hammer;
	private bool activated = false;
	private bool finished = false;

	private float startRotation;
	private float endRotation;

	private float speed = 1f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
	}

	public async void ActivateTrap()
	{
		float delta = 1 / 60f;
		
		while (!finished)
		{
			hammer.RotationDegrees += (float)delta * speed;
			speed = Mathf.Min(300, speed * 2f);
			

			if (hammer.RotationDegrees > endRotation)
			{
				hammer.RotationDegrees = endRotation;
				finished = true;
			}
			await Task.Delay(TimeSpan.FromSeconds(delta));
		}
	}

	public override void Setup(ObstacleSpot spot)
	{
		base.Setup(spot);
		hammer.Position = spot.ceilingSpot.Position + new Vector2(0, 12);
		hammer.Rotation = spot.ceilingSpot.Rotation;
		startRotation = hammer.RotationDegrees;
		endRotation = hammer.RotationDegrees + 180;
	}
}
