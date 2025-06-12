using Godot;
using System;

public partial class LevelSelect : Window
{
	[Export] public Button Level1Button;
	[Export] public Button Level2Button;
	[Export] public Button Level3Button;
	[Export] public Button Level4Button;
	[Export] public Button EndlessButton;
	[Export] public Button StartButton;
	public override void _Ready()
	{
		// Connect button signals to their respective methods
		Level1Button.Pressed += OnLevel1ButtonPressed;
		Level2Button.Pressed += OnLevel2ButtonPressed;
		Level3Button.Pressed += OnLevel3ButtonPressed;
		Level4Button.Pressed += OnLevel4ButtonPressed;
		EndlessButton.Pressed += OnEndlessButtonPressed;
		StartButton.Pressed += OnStartButtonPressed;
		CloseRequested += () => { GetTree().Quit(); };
		base._Ready();

		Unresizable = true;
		SharpCorners = true;
		Title = "Level Selection";
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public void OnLevel1ButtonPressed()
	{
		SpeedRunTimer.Instance.StartTimer();
		GameManager.State = GameManager.GameState.Level1;
		Quit();
	}
	public void OnLevel2ButtonPressed()
	{
		SpeedRunTimer.Instance.StartTimer();
		GameManager.State = GameManager.GameState.Level2;
		Quit();
	}
	public void OnLevel3ButtonPressed()
	{
		SpeedRunTimer.Instance.StartTimer();
		GameManager.State = GameManager.GameState.Level3;
		Quit();
	}
	public void OnLevel4ButtonPressed()
	{
		SpeedRunTimer.Instance.StartTimer();
		GameManager.State = GameManager.GameState.FinalLevel;
		Quit();
	}
	public void OnEndlessButtonPressed()
	{
		GameManager.State = GameManager.GameState.IntroGameEndless;
		Quit();
	}
	public void OnStartButtonPressed()
	{
		GameManager.State = GameManager.GameState.IntroGame;
		GameManager.HaveLaunchedTheGameFromTheStart = true;
		Quit();
	}

	public void Quit()
	{
		this.QueueFree();
	}
}
