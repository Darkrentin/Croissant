using Godot;

public partial class Level2 : Node2D
{
	[Export] public WaveManager WaveManager;
	[Export] public Label ExportScoreLabel { get => ScoreLabel; set => ScoreLabel = value; }
	[Export] public AnimationPlayer ExportAnimationPlayer { get => AnimationPlayer; set => AnimationPlayer = value; }
	public static Label ScoreLabel;
	public static AnimationPlayer AnimationPlayer;
	public static int WaveNum = 0;
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
		AddWave();
	}

	public void NextLvl()
	{
		GameManager.State = GameManager.GameState.HelperDialogue1;
		QueueFree();
	}

	public static void AddWave()
	{
		WaveNum++;
		ScoreLabel.Text = WaveNum.ToString();
		AnimationPlayer.Play("ScoreUp");
	}
}