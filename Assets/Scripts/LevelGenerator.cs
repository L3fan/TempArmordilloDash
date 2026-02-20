using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Transactions;

public partial class LevelGenerator : Level
{
	[Export] public int sectionCount = 5;
	[Export] public String sectionFolderPath;
	[Export] public PackedScene[] sections;
	[Export] public String endSectionPath;
	[Export] public Node2D endPointNode;
	private Vector2 currentStartPoint;
	public List<PackedScene> commonSectionPool;
	public List<PackedScene> uncommonSectionPool;
	public List<PackedScene> rareSectionPool;
	
	private const int COMMON_SECTION_CHANCE = 50;
	private const int UNCOMMON_SECTION_CHANCE = 95;
	private const int RARE_SECTION_CHANCE = 100;

	private int lastRarityGenerated = 0;

	private const float MAX_HEIGHT = 2000;
	private float currentHeight = 0;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		currentStartPoint = endPointNode.Position;

		sectionFolderPath = Settings.Instance.levelSectionsFolderPath;

		endSectionPath = Settings.Instance.levelEndSectionPath;
		
		GetSectionPaths();

		CreateSectionPools();
		
		for (int i = 0; i < sectionCount; i++)
		{
			Random random = new Random();

			int randomRarity = GenerateRandomRarity(random);
			lastRarityGenerated = randomRarity;
			
			List<PackedScene> sectionPool = GetRaritySectionPool(randomRarity);

			int randIndex = GenerateRandomIndex(random, sectionPool);
			//GD.Print("Randomly Generating number " + randIndex);
			LevelSection randomLevelSection = sectionPool[randIndex].Instantiate() as LevelSection;
			randomLevelSection.Position = currentStartPoint - randomLevelSection.startPointNode.Position;
			currentHeight += randomLevelSection.GetYDifference();
			AddChild(randomLevelSection);
			currentStartPoint = randomLevelSection.endPointNode.GlobalPosition;
		}
		var endSection = GD.Load<PackedScene>(endSectionPath);
		LevelSection endSectionScene = endSection.Instantiate() as LevelSection;
		endSectionScene.Position = currentStartPoint - endSectionScene.startPointNode.Position;
		AddChild(endSectionScene);
	}

	private int GenerateRandomIndex(Random random, List<PackedScene> sectionPool)
	{
		int randIdx = random.Next(sectionPool.Count);
		
		bool retry = CheckForHeight(randIdx, sectionPool);
		while (retry)
		{
			randIdx = random.Next(sectionPool.Count);
			retry = CheckForHeight(randIdx, sectionPool);
		}
		
		return randIdx;
	}

	private bool CheckForHeight(int randIdx, List<PackedScene> sectionPool)
	{
		LevelSection randLevelSect = sectionPool[randIdx].Instantiate() as LevelSection;
		float yDiff = randLevelSect.GetYDifference();
		bool result = yDiff + currentHeight > MAX_HEIGHT || yDiff + currentHeight < -MAX_HEIGHT;
		return result;
	}

	private int GenerateRandomRarity(Random random)
	{
		int randomRarity = random.Next(100);
		while (CanGenerateRareSection(randomRarity))
			randomRarity = random.Next(100);
		return randomRarity;
	}

	private bool CanGenerateRareSection(int rarity)
	{
		return rarity == RARE_SECTION_CHANCE && lastRarityGenerated == RARE_SECTION_CHANCE;
	}

	private List<PackedScene> GetRaritySectionPool(int chanceResult)
	{
		//GD.Print("Chance: " + chanceResult);
		switch (chanceResult)
		{
			case var n when n < COMMON_SECTION_CHANCE:
				return commonSectionPool;
			case var n when n < UNCOMMON_SECTION_CHANCE:
				return uncommonSectionPool;
			case var n when n < RARE_SECTION_CHANCE:
				return rareSectionPool;
			
		}
		GD.Print("Couldnt add any section");

		return commonSectionPool;
	}

	private void CreateSectionPools()
	{
		commonSectionPool = new List<PackedScene>();
		uncommonSectionPool = new List<PackedScene>();
		rareSectionPool = new List<PackedScene>();

		foreach (var section in sections)
		{
			LevelSection levelSectInstance = section.Instantiate() as LevelSection;
			
			if (levelSectInstance == null)
				continue;
			
			switch (levelSectInstance.rarity)
			{
				case SectionRarity.COMMON:
					commonSectionPool.Add(section);
					break;
				case SectionRarity.UNCOMMON:
					uncommonSectionPool.Add(section);
					break;
				case SectionRarity.RARE:
					rareSectionPool.Add(section);
					break;
			}
		}
		
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
