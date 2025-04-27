using Godot;

public partial class Bsod : Window
{
	[Export] public Button RestartButton;

	public override void _Ready()
	{
		RestartButton.Pressed += () => { GameManager.State = GameManager.GameState.IntroHelper; };
	}

	public override void _Process(double delta)
	{

	}
}