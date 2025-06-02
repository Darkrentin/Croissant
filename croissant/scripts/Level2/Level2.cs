using Godot;

public partial class Level2 : Node2D
{
	[Export] public WaveManager WaveManager;
	public static CursorWindow CursorWindow;
	public static Level2 Instance;
	private bool isFrozen = false;

	public override void _Ready()
	{
		Instance = this;
		if (CursorWindow == null)
		{
			CursorWindow = States.CursorWindowScene.Instantiate<CursorWindow>();
			GameManager.GameRoot.AddChild(CursorWindow);
		}
		WaveManager.EndWave += EndActions;
	}

	public void EndActions()
	{
		CursorWindow.ClearAllDots();
		GameManager.State = GameManager.GameState.Dialogue2;
		CursorWindow.GetParent().RemoveChild(CursorWindow);
		CursorWindow.QueueFree();
		GetParent().RemoveChild(this);
		VisualCollision.End = true;
		QueueFree();
	}

	public void FreezeLevel2()
	{
		isFrozen = true;
	}

	public void UnfreezeLevel2()
	{
		isFrozen = false;
	}

	public bool IsFrozen => isFrozen;
}
