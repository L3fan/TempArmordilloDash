using Godot;
using System;
using System.Collections.Generic;

public partial class DashTrail : Node2D
{
	[Export] public Player player;
	private List<Sprite2D> trailPieces = new List<Sprite2D>();
	private int lastTrailPiece = 0;
	private float trailOffset = 0;
	private float minDistance = 100f;
		
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Node n in GetChildren())
		{
			if(n.Name.ToString().StartsWith("Trail") && n is Sprite2D)
				trailPieces.Add((Sprite2D)n);
		}
		GD.Print(trailPieces.Count);
	}

	public override void _PhysicsProcess(double delta)
	{
		int i = 0;
		foreach (Sprite2D trailPiece in trailPieces)
		{
			Color color = trailPiece.Modulate;
			if (color.A != 0)
			{
				trailPiece.Modulate = new Color(color.R, color.G, color.B, Math.Max(color.A - 2f * (float)delta, 0f));
				i++;
			}
		}

		lastTrailPiece = i;
		
		if(trailOffset > 0)
			trailOffset -= (float)delta;

		if (player.IsDashing() && trailOffset <= 0 && PlayerHasMovedFarEnough())
		{
			bool foundAvailablePiece = false;
			foreach (Sprite2D trailPiece in trailPieces)
			{
				if (foundAvailablePiece)
					continue;
				
				Color color = trailPiece.Modulate;
				if (color.A != 0)
					continue;

				trailPiece.Modulate = new Color(color.R, color.G, color.B, 0.5f);

				Sprite2D playerSprite = player.GetSprite();
				trailPiece.Texture = playerSprite.Texture;
				trailPiece.Vframes = playerSprite.Vframes;
				trailPiece.Hframes = playerSprite.Hframes;
				
				trailPiece.Frame = playerSprite.Frame;
				trailPiece.Position = player.Position;
				trailPiece.Rotation = player.GetSprite().Rotation;
				trailPiece.Scale = playerSprite.Scale;
				
				foundAvailablePiece = true;
				trailOffset = 0.05f;
			}
		}
	}

	private bool PlayerHasMovedFarEnough()
	{
		return (player.Position - trailPieces[lastTrailPiece].Position).Length() >= minDistance;
	}
}
