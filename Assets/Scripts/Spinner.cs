using Godot;

namespace Armordillo_Dash.Assets.Scripts;

public partial class Spinner : Obstacle
{
	public AnimationPlayer animationPlayer;

	private bool test = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public override void _PhysicsProcess(double delta)
	{
	}

	public override void Setup(ObstacleSpot spot)
	{
		base.Setup(spot);
		animationPlayer = GetChild(1).GetNode<AnimationPlayer>("AnimationPlayer");
		animationPlayer.CurrentAnimation = "Spin";
	}
}