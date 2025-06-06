using Godot;
using System;

public partial class LevelSelect : FloatWindow
{
	// Called when the node enters the scene tree for the first time.
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
		base._Ready();

		Minimizable = false;
		Unresizable = true;
		Size = Lib.GetAspectFactor(Size);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		// Additional processing can be done here if needed
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
		SpeedRunTimer.Instance.Visible = false;
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
