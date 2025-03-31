using Godot;
using System;

public partial class Level2 : Node2D
{
	[Export] public CursorWindow CursorWindow;

	public static Level2 Instance;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Instance = this;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
