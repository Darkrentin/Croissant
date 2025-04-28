using Godot;

public partial class Level2 : Node2D
{
	[Export] public CursorWindow CursorWindow;
	[Export] public WaveManager WaveManager;
	public static Level2 Instance;

	public override void _Ready()
	{
		Instance = this;
	}

	public override void _Process(double delta)
	{
		const int MaxWave = 10;
		if (WaveManager.CurrentWave > MaxWave)
		{
			GameManager.State = GameManager.GameState.HelperDialogue1;
			QueueFree();
		}
	}
}