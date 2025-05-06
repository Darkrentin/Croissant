using Godot;

public partial class Level2 : Node2D
{
	public static CursorWindow CursorWindow;
	[Export] public WaveManager WaveManager;
	public static Level2 Instance;

	public override void _Ready()
	{
		Instance = this;
		if (CursorWindow == null)
		{
			CursorWindow = States.CursorWindowScene.Instantiate<CursorWindow>();
			GameManager.GameRoot.AddChild(CursorWindow);
		}
		WaveManager.EndWave += NextLvl;
	}

	public override void _Process(double delta)
	{
	}

	public void NextLvl()
	{
		GameManager.State = GameManager.GameState.HelperDialogue1;
		QueueFree();
	}
}