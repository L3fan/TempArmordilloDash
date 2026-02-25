using Godot;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Armordillo_Dash.Assets.Scripts;

public partial class Settings : Node2D
{
	public static Settings Instance { get; private set; }
	[Export] public String mainMenuScenePath;
	[Export] public String playerScenePath;
	[Export] public String trailScenePath;
	[Export] public String levelUIScenePath;
	[Export] public String obstaclesFolderPath;
	public String levelScenesFolderPath;
	public String levelSectionsFolderPath;
	public String levelEndSectionPath;

	private List<PackedScene> commonObs = new List<PackedScene>();
	private List<PackedScene> uncommonObs = new List<PackedScene>();
	private List<PackedScene> rareObs = new List<PackedScene>();

	public SettingsSave settingsSave;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
		String sceneFolder = "res://Assets/Scenes/";
		
		mainMenuScenePath = sceneFolder + "MainMenuScene.tscn";
		
		playerScenePath = sceneFolder + "Player.tscn";
		
		trailScenePath = sceneFolder + "DashTrail.tscn";

		levelUIScenePath = sceneFolder + "LevelUI.tscn";
		
		obstaclesFolderPath = sceneFolder + "Obstacles/";
		
		levelScenesFolderPath = sceneFolder + "Levels/";
		
		levelSectionsFolderPath = levelScenesFolderPath + "LevelSections/";
		
		levelEndSectionPath = sceneFolder + "EndLevelSection.tscn";

		LoadObstacles();

		LoadSettings();
		
		GD.Print("Loaded Settings");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void LoadObstacles()
	{
		List<PackedScene> obstacleScenes = new List<PackedScene>();
		var dir = DirAccess.Open(obstaclesFolderPath);
		if (dir != null)
		{
			dir.ListDirBegin();
			int count = 0;
			String fileName = dir.GetNext();
			while (fileName != String.Empty)
			{
				if (fileName.GetExtension() == "remap")
					fileName = fileName.Substring(0, fileName.Length - 6);
				
				if (fileName.GetExtension() == "tscn")
				{
					String fullPath = obstaclesFolderPath.PathJoin(fileName);
					obstacleScenes.Add(GD.Load<PackedScene>(fullPath));
				}

				fileName = dir.GetNext();
				count++;
			}
			GD.Print("Found " + count + " Obstacle files.");
		}
		else
		{
			GD.Print("An error has occured trying to access a path.");
		}

		foreach (PackedScene obstacleScene in obstacleScenes)
		{
			Obstacle obstacle = obstacleScene.Instantiate() as Obstacle;
			switch (obstacle.Rarity)
			{
				case ObstacleRarity.COMMON:
					commonObs.Add(obstacleScene);
					break;
				case ObstacleRarity.UNCOMMON:
					uncommonObs.Add(obstacleScene);
					break;
				case ObstacleRarity.RARE:
					rareObs.Add(obstacleScene);
					break;
			}
			obstacle.QueueFree();
		}
	}

	public Obstacle GetRandomObstacle()
	{
		Random random = new Random();
		List<PackedScene> chosenObstacles = new List<PackedScene>();
		switch (random.Next(0, 100))
		{
			case var n when n < 50:
				chosenObstacles = commonObs;
				break;
			case var n when n < 85:
				chosenObstacles = uncommonObs;
				break;
			case var n when n < 100:
				chosenObstacles = rareObs;
				break;
		}
		
		Obstacle randomObstacle = null;
		if(chosenObstacles.Count > 0)
			randomObstacle = chosenObstacles[random.Next(0, chosenObstacles.Count)].Instantiate() as Obstacle;
		
		return randomObstacle;
	}

	public void LoadSettings()
	{
		if (FileAccess.FileExists("user://settings.save"))
		{

			var settingsFile = FileAccess.Open("user://settings.save", Godot.FileAccess.ModeFlags.Read);

			String jsonString = settingsFile.GetAsText();

			var json = new Json();
			var parseResult = json.Parse(jsonString);
			if (parseResult != Error.Ok)
			{
				GD.PrintErr("Error parsing settings: " + json.GetErrorMessage() + " at line " +
				            json.GetErrorLine());
			}

			settingsSave = JsonSerializer.Deserialize<SettingsSave>(jsonString);

			settingsFile.Close();
		}

		if (settingsSave == null)
		{
			VolumeSettings defaultVolSet = new VolumeSettings();
			defaultVolSet.MasterVolume = 0.7f;
			defaultVolSet.MusicVolume = 0.7f;
			defaultVolSet.SFXVolume = 0.7f;
			settingsSave = new SettingsSave();
			settingsSave.VolumeSettings = defaultVolSet;
		}
	}

	public void SaveSettings()
	{
		var saveFile = FileAccess.Open("user://settings.save", FileAccess.ModeFlags.Write);

		var jsonString = JsonSerializer.Serialize(settingsSave);

		saveFile.StoreLine(jsonString);

		saveFile.Close();

		SettingsSave setSave = JsonSerializer.Deserialize<SettingsSave>(jsonString);
	}
}

[Serializable]
public class VolumeSettings
{
	public float MasterVolume { get; set; }
	public float MusicVolume { get; set; }
	public float SFXVolume { get; set; }
}

[Serializable]
public class SettingsSave
{
	public VolumeSettings VolumeSettings { get; set; }
	
}
