using Godot;

public partial class Level2 : Node2D
{
	[Export] public WaveManager WaveManager;
	public static CursorWindow CursorWindow;
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

	public void NextLvl()
	{
		GameManager.State = GameManager.GameState.HelperDialogue1;
		QueueFree();
	}

	
}