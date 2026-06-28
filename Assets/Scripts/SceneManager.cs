using Godot;
using System;
using System.Collections;
using System.Threading.Tasks;

public partial class SceneManager : Control
{
	[Export] public Node currentScene { get; private set; }

	public static SceneManager Instance;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(Instance != null)
			QueueFree();

		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void LoadScene(string resourcePath)
	{
		//unload current Scene
		currentScene.QueueFree();
		
		//load new Scene
		PackedScene sceneToLoad = GD.Load<PackedScene>(resourcePath);
		currentScene = sceneToLoad.Instantiate();
		AddChild(currentScene);
	}
	
	
	public void LoadScene(PackedScene scene)
	{
		//unload current Scene
		currentScene.QueueFree();
		
		//load new Scene
		currentScene = scene.Instantiate();
		AddChild(currentScene);
	}
}
