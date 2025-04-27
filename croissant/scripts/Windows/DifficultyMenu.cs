using Godot;

public partial class DifficultyMenu : FloatWindow
{
	[Export] Button PlayButton;
	[Export] OptionButton Difficulty;

	public override void _Ready()
	{
		PlayButton.Pressed += StartGame;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2;
	}

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
				GameManager.SetDifficulty(DifficultyLevel.Normal);
				break;
			case 2:
				GameManager.SetDifficulty(DifficultyLevel.Hard);
				break;
		}
		GameManager.State = GameManager.GameState.IntroGame;
		QueueFree();
	}
}