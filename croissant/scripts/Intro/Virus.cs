using Godot;
using System;

public partial class Virus : FloatWindow
{
	[Export] Camera3D Camera;
	[Export] Node3D Computer;

	Vector2I screenSize = DisplayServer.ScreenGetSize();

	public override void _Ready()
	{
		base._Ready();
		Position = Lib.GetScreenPosition(0.25f, 0.25f);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		Computer.RotateY(1 * (float)delta);

		//Movement Fiesta
		//StartExponentialTransition(Lib.GetScreenPosition(Lib.GetRandomNormal(0, 1), Lib.GetRandomNormal(0, 1)), 1f);
	}
}
