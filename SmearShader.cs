using Godot;
using System;

public partial class SmearShader : Control
{
	[Export] private Sprite2D sprite;
	[Export] private Shader shader;
	void Ready()
	{
		shader = GetNode<Shader>(".");
	}

	void _Process(double _delta)
	{
		
	}
}
