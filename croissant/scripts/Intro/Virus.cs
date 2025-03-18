using Godot;
using System;

public partial class Virus : FloatWindow
{
	[Export] Camera3D Camera;
	[Export] MeshInstance3D Mesh;

	Vector2I screenSize = DisplayServer.ScreenGetSize();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Position = Lib.GetScreenPosition(0.25f, 0.25f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		Mesh.RotateY(3 * (float)delta);
		StartExponentialTransition(Lib.GetScreenPosition(Lib.GetRandomNormal(0, 1), Lib.GetRandomNormal(0, 1)), 1f);
	}
}
