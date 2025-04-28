using Godot;

public partial class Level2 : Node2D
{
	public static CursorWindow CursorWindow;
	[Export] public WaveManager WaveManager;
	public static Level2 Instance;

	public override void _Ready()
	{
		Instance = this;
		if(CursorWindow == null)
		{
			CursorWindow = States.CursorWindowScene.Instantiate<CursorWindow>();
			GameManager.GameRoot.AddChild(CursorWindow);
		}
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