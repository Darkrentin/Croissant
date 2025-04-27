using Godot;
using System;

public partial class Bsod : Window
{
	[Export] public Button RestartButton;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		RestartButton.Pressed += () =>
		{
			GameManager.State = GameManager.GameState.IntroHelper;
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
