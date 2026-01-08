using Godot;
using System;

public partial class LevelGenerator : Level
{
	[Export] public int sectionCount = 5;
	[Export] public String[] sectionPaths;
	[Export] public String endSectionPath;
	[Export] public Node2D endPointNode;
	private Vector2 currentStartPoint;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentStartPoint = endPointNode.Position;
		for (int i = 0; i < sectionCount; i++)
		{
			Random random = new Random();
			int randIndex = random.Next(sectionPaths.Length);
			var scene = GD.Load<PackedScene>(sectionPaths[randIndex]);
			LevelSection randomLevelSection = scene.Instantiate() as LevelSection;
			randomLevelSection.Position = currentStartPoint - randomLevelSection.startPointNode.Position;
			AddChild(randomLevelSection);
			currentStartPoint = randomLevelSection.endPointNode.GlobalPosition;
		}
		var endSection = GD.Load<PackedScene>(endSectionPath);
		LevelSection endSectionScene = endSection.Instantiate() as LevelSection;
		endSectionScene.Position = currentStartPoint - endSectionScene.startPointNode.Position;
		AddChild(endSectionScene);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
