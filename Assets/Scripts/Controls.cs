using Godot;
using System;

public abstract class Controls
{
	public abstract void InputCalc(float delta);
	public abstract void SlowdownCalc(float delta);
	public abstract void DashCalc(float delta);
}
