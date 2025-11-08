using Godot;
using System;

public partial class SmearShader : Control
{

	private Viewport bufferA;
	private Viewport bufferB;
	private TextureRect display;
	ShaderMaterial shaderA; 
	ShaderMaterial shaderB;

	private bool useA = true;


	void Ready()
	{
		bufferA = GetNode<Viewport>("BufferA");
		bufferB	= GetNode<Viewport>("BufferB");

		shaderA = (ShaderMaterial)GetNode<TextureRect>("BufferA").Material;
		shaderB = (ShaderMaterial)GetNode<TextureRect>("BufferB").Material;

		Vector2 screenSize = GetViewport().GetVisibleRect().Size;
		bufferA.GetWindow().Size = new Vector2I((int)screenSize.X, (int)screenSize.Y);
		bufferB.GetWindow().Size = new Vector2I((int)screenSize.X, (int)screenSize.Y);

		shaderA.SetShaderParameter("previous_buffer", bufferB.GetTexture());
		shaderB.SetShaderParameter("previous_buffer", bufferA.GetTexture());
	}

	void _Process(double _delta)
	{
		Vector2 mouseUV = GetViewport().GetMousePosition() / bufferA.GetWindow().Size;

		shaderA.SetShaderParameter("mouse_pos", mouseUV);
		shaderB.SetShaderParameter("mouse_pos", mouseUV);

		bool pressed = Input.IsMouseButtonPressed(MouseButton.Left);
		shaderA.SetShaderParameter("mouse_pressed", pressed);
		shaderB.SetShaderParameter("mouse_pressed", pressed);

		if (useA)
		{
			bufferA.VrsUpdateMode = Viewport.VrsUpdateModeEnum.Always;
			bufferB.VrsUpdateMode = Viewport.VrsUpdateModeEnum.Disabled;
			display.Texture = bufferA.GetTexture();
		}
		else
		{
			bufferB.VrsUpdateMode = Viewport.VrsUpdateModeEnum.Always;
			bufferA.VrsUpdateMode = Viewport.VrsUpdateModeEnum.Disabled;
			display.Texture = bufferB.GetTexture();
		}

		useA = !useA;
	}
}
