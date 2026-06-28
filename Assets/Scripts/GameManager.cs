using Godot;
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using FmodSharp;

public partial class GameManager : Node
{
	public delegate void DemoToggleEvent();

	public static GameManager Instance { get; private set; }
	
	[Export] public bool debugLevel = false;
	public SceneType currentSceneType = SceneType.None;
	public Node currentScene = null;
	public Player player;
	private int startTime;
	public int totalTime;
	public AudioStreamPlayer sfxPlayer;

	private PackedScene mainMenuPackedScene = null;
	private PackedScene levelPackedScene = null;

	public bool demo = false;
	public event DemoToggleEvent onToggleDemo;
	
	private readonly List<FmodBank> _loadedBanks = [];
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		onToggleDemo += OnToggleDemo;
		ProcessMode = ProcessModeEnum.Always;
		Instance = this;

		//Load(SceneType.MainMenu);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void StartTimer()
	{
		startTime = (int)Time.GetTicksMsec();
		
		GD.Print("Level Start Time: " + startTime);
	}

	public void GameOver()
	{
		//GD.Print("Game Over");
		if(SceneManager.Instance.currentScene is not Level) return;
		
		GetTree().Paused = true;
		Level level = (Level)SceneManager.Instance.currentScene;
		Node2D resultScreen = level.levelUI.camera.resultScreen;
		resultScreen.Visible = true;
		Label timeLabel = resultScreen.GetChild(0).GetChild<Label>(1);
		int finishTime = (int)Time.GetTicksMsec();
		totalTime = finishTime - startTime;
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
		FmodServerWrapper.GetAllBanks();
		currentSceneType = sceneType;
	}

	public void LoadMainMenu()
	{
		GetTree().Paused = false;
		SceneManager.Instance.LoadScene(Settings.Instance.mainMenuScenePath);
	}

	public void LoadLevel()
	{
		if (debugLevel)
		{
			SceneManager.Instance.LoadScene(LevelManager.Instance.GetLevelScene(LevelType.Debug));
		}
		else
		{
			SceneManager.Instance.LoadScene(LevelManager.Instance.GetLevelScene(LevelType.Default));
		}
	}

	public void UpdateVolume(float volume, string busName)
	{
		int busIndex = AudioServer.GetBusIndex(busName);
		AudioServer.SetBusVolumeLinear(busIndex, volume);
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.X)
			{
				if (Input.IsKeyPressed(Key.Ctrl) && Input.IsKeyPressed(Key.Shift))
					onToggleDemo?.Invoke();
			}
		}
	}

	private void OnToggleDemo()
	{
		demo = !demo;
	}
}

public enum SceneType
{
	MainMenu,
	Level,
	None
}
