using Godot;
using System;
using System.Xml.Linq;

public partial class GameManager : Node
{
	public static GameManager Instance { get; private set; }
	
	[Export] public bool debugLevel = false;
	public SceneType currentSceneType = SceneType.None;
	public Node currentScene = null;
	public Player player;
	public int totalTime;

	private PackedScene mainMenuPackedScene = null;
	private PackedScene levelPackedScene = null;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		Instance = this;
		
		//Load(SceneType.MainMenu);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void GameOver()
	{
		GD.Print("Game Over");
		if(GetTree().GetCurrentScene() is not Level) return;
		
		GetTree().Paused = true;
		Level level = (Level)GetTree().GetCurrentScene();
		Node2D resultScreen = level.levelUI.camera.resultScreen;
		resultScreen.Visible = true;
		Label timeLabel = resultScreen.GetChild(0).GetChild<Label>(1);
		int finishTime = (int)Time.GetTicksMsec();
		totalTime = finishTime - level.GetStartTime();
		timeLabel.Text = IntToTime(totalTime);
		
		//GD.Print(totalTime);
	}

	public string IntToTime(int totalTime)
	{
		String minutes = (totalTime / (1000 * 60)).ToString();
		if(minutes.Length < 2) minutes = "0" + minutes;
		
		String seconds = (totalTime % (1000 * 60) / 1000).ToString();
		if(seconds.Length < 2) seconds = "0" + seconds;
		
		String miliseconds = (totalTime % (1000 * 60) % 1000).ToString();
		while(miliseconds.Length < 3) miliseconds = "0" + miliseconds;
		
		return minutes + ":" + seconds + ":" + miliseconds;
	}

	public void Load(SceneType sceneType)
	{
		//load Scene
		switch (sceneType)
		{
			case SceneType.MainMenu:
				LoadMainMenu();
				break;
			case SceneType.Level:
				LoadLevel();
				break;
		}
		currentSceneType = sceneType;
	}

	public void LoadMainMenu()
	{
		GetTree().Paused = false;
		mainMenuPackedScene = GD.Load<PackedScene>(Settings.Instance.mainMenuScenePath);
		GetTree().ChangeSceneToPacked(mainMenuPackedScene);
	}

	public void LoadLevel()
	{
		if (debugLevel)
		{
			GetTree().ChangeSceneToPacked(LevelManager.Instance.GetLevelScene(LevelType.Debug));
		}
		else
		{
			GetTree().ChangeSceneToPacked(LevelManager.Instance.GetLevelScene(LevelType.Default));
		}
	}

	public void UpdateVolume(float volume, string busName)
	{
		int busIndex = AudioServer.GetBusIndex(busName);
		AudioServer.SetBusVolumeLinear(busIndex, volume);
	}
}

public enum SceneType
{
	MainMenu,
	Level,
	None
}
