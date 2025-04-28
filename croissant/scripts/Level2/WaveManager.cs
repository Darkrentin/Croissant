using Godot;

public partial class WaveManager : Node
{
	[Export] public PackedScene[] EnemyWindows;
	[Export] public int[] EnimyWindowsWeights;
	[Export] int NumberOfEnemy;
	[Export] Node SpawnNode;
	private Timer WaveResetTimer;
	[Export] public Vector2 WaveResetTime = new Vector2(1, 3);
	public int CurrentWave = 1;
	public int CurrentWaveEnemy = -1;
	[Export] public int CurrentWaveMaxEnemy = 6;
	public int CurrentWavePoints = 0;
    [Export] public int CurrentWaveMaxPoints = 5;
	[Export] public int IncreasePoints = 1;

	public override void _Ready()
	{
		WaveResetTimer = new Timer();
		WaveResetTimer.WaitTime = WaveResetTime.X;
		WaveResetTimer.OneShot = true;
		WaveResetTimer.Timeout += StartWave;
		AddChild(WaveResetTimer);
		WaveResetTimer.Start();
	}

	public override void _Process(double delta)
	{
		if (CurrentWaveEnemy == 0)
		{
			CurrentWaveMaxPoints += IncreasePoints;
			CurrentWaveEnemy = CurrentWaveMaxEnemy;
			CurrentWavePoints = 0;
			//Lib.Print($"Starting wave {CurrentWave} with {CurrentWaveMaxEnemy} enemies");
			WaveResetTimer.WaitTime = Lib.GetRandomNormal(WaveResetTime.X, WaveResetTime.Y);
			WaveResetTimer.Start();
		}
	}

	public void StartWave()
	{
		CurrentWave++;
		CurrentWaveEnemy = 0;
		while (CurrentWavePoints < CurrentWaveMaxPoints)
		{
			if (CurrentWaveEnemy < CurrentWaveMaxEnemy)
			{
				SpawnWindow();
			}
			else
			{
				//Lib.Print($"Enemy weight {enemyWeight} is too high for current wave {CurrentWave}");
			}
		}
	}

	public void SpawnWindow()
	{
		int enemyIndex = Lib.rand.Next(0, EnemyWindows.Length);
		int enemyWeight = EnimyWindowsWeights[enemyIndex];
		CurrentWavePoints += enemyWeight;
		CurrentWaveEnemy++;
		Window enemyWindow = EnemyWindows[enemyIndex].Instantiate<Window>();
		SpawnNode.AddChild(enemyWindow);
	}

	public void EnemyDefeated()
	{
		CurrentWaveEnemy--;
	}
}