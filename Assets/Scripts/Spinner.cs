using System;
using System.Threading.Tasks;
using Godot;

namespace Armordillo_Dash.Assets.Scripts;

public partial class Spinner : Obstacle
{
	public AnimationPlayer animationPlayer;

	private bool test = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("%AnimationPlayer");
		animationPlayer.Pause();
		PlayAfterSeconds(Random.Shared.Next(0, 1));
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

	public async void PlayAfterSeconds(float seconds)
	{
		await Task.Delay(TimeSpan.FromSeconds(seconds));
		animationPlayer.Play("Spin");
	}
}