using Godot;
using System;
using System.Diagnostics;

public partial class DifficultyMenu : FloatWindow
{
	[Export] Button PlayButton;
	[Export] OptionButton Difficulty;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PlayButton.Pressed += StartGame;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartGame()
	{
		switch (Difficulty.Selected)
		{
			case 0:
				GameManager.SetDifficulty(DifficultyLevel.Easy);
				break;
			case 1:
				GameManager.SetDifficulty(DifficultyLevel.Medium);
				break;
			case 2:
				GameManager.SetDifficulty(DifficultyLevel.Hard);
				break;
		}
		GameManager.State = GameManager.GameState.IntroGame;
		QueueFree();
	}
}
