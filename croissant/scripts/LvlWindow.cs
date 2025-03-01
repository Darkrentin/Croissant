using Godot;
using System;

public partial class LvlWindow : Window
{

	[Export] public Camera2D Camera;

	public Vector2I LastPos = Vector2I.Zero;
	public Vector2I Velocity = Vector2I.Zero;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Camera.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Velocity = Position - LastPos;
		LastPos = Position;
		Camera.Position = Position;
		World2D = GameManager.MainWindow.GetWorld2D();
	}
}
