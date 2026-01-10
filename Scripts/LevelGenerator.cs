using Godot;
using System;
using System.Collections.Generic;
using System.Transactions;

public partial class LevelGenerator : Level
{
	[Export] public int sectionCount = 5;
	[Export] public String sectionFolderPath;
	[Export] public PackedScene[] sections;
	[Export] public String endSectionPath;
	[Export] public Node2D endPointNode;
	private Vector2 currentStartPoint;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		currentStartPoint = endPointNode.Position;

		sectionFolderPath = "res://Scenes/Levels/LevelSections/";
		
		GetSectionPaths();
		
		for (int i = 0; i < sectionCount; i++)
		{
			Random random = new Random();
			int randIndex = random.Next(sections.Length);
			LevelSection randomLevelSection = sections[randIndex].Instantiate() as LevelSection;
			randomLevelSection.Position = currentStartPoint - randomLevelSection.startPointNode.Position;
			AddChild(randomLevelSection);
			currentStartPoint = randomLevelSection.endPointNode.GlobalPosition;
		}
		var endSection = GD.Load<PackedScene>(endSectionPath);
		LevelSection endSectionScene = endSection.Instantiate() as LevelSection;
		endSectionScene.Position = currentStartPoint - endSectionScene.startPointNode.Position;
		AddChild(endSectionScene);
	}

	private void GetSectionPaths()
	{
		List<PackedScene> tempList = new List<PackedScene>();
		var dir = DirAccess.Open(sectionFolderPath);
		dir.ListDirBegin();
		String fileName = dir.GetNext();
		while (fileName != String.Empty)
		{
			if (fileName.EndsWith(".tscn"))
				tempList.Add(GD.Load<PackedScene>(sectionFolderPath + fileName));
			fileName = dir.GetNext();
		}
		sections = tempList.ToArray();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
