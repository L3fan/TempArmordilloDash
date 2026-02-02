using Godot;
using System;

public partial class CameraMovement : Camera2D
{
	[Export] public Player player;

	[Export] public Vector2 bufferZone;

	[Export] public ColorRect slowDownTint;

	[Export] public ColorRect blackScreen;

	[Export] public Node2D resultScreen;

	[Export] public float maxCameraDistance = 200;
	[Export] public float maxCameraMoveDistance = 160;
	[Export] public float maxCameraZoomDistance = 10;

	private Vector2 startZoom;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Position = player.Position;
		startZoom = Zoom;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
			
		//Set Slowdown Tint
		if(player.IsInSlowDown())
			slowDownTint.Color = new Color(slowDownTint.Color.R, slowDownTint.Color.G, slowDownTint.Color.B, 0.25f);
		else
			slowDownTint.Color = new Color(slowDownTint.Color.R, slowDownTint.Color.G, slowDownTint.Color.B, 0);
	}

	public override void _PhysicsProcess(double delta)
	{
		//Camera Movement
		Vector2 offset = CalculateOffset() * Vector2.Right;
		Vector2 target = player.Position + offset - new Vector2(0, 200);
		Vector2 moveDirection = (target - Position) / 2f;
		Position += moveDirection.Normalized() * Mathf.Sqrt(Mathf.Min(Mathf.Pow(moveDirection.Length(), 2), Mathf.Pow(maxCameraMoveDistance, 2)));

		Zoom = Zoom.Lerp(startZoom - Vector2.One * offset.Length() / maxCameraDistance / 6f, (float)delta * maxCameraZoomDistance);
	}

	private Vector2 CalculateOffset()
	{
		if(player.Velocity.Length()/10 <= maxCameraDistance)
			return player.Velocity/10;
		
		return player.Velocity.Normalized() * maxCameraDistance;
	}

	public void DarkenScreen(float amount)
	{
		blackScreen.Color = new Color(blackScreen.Color.R, blackScreen.Color.G, blackScreen.Color.B, blackScreen.Color.A + amount);
	}
	
	public bool NoBlackScreen()
	{
		return blackScreen.Color.A <= 0.0f;
	}

	public bool ScreenIsBlack()
	{
		return blackScreen.Color.A >= 1.0f;
	}
}
