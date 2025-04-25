using Godot;
using System;

public partial class WaveManager : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export] public PackedScene[] EnemyWindows;
	[Export] public int[] EnimyWindowsWeights;
	[Export] int NumberOfEnemy;
	[Export] Node SpawnNode;

	public int CurrentWave = 0;
	public int CurrentWaveEnemy = -1;
	public int CurrentWaveMaxEnemy = 6;
	public int CurrentWavePoints = 0;
	public int CurrentWaveMaxPoints = 5;

	Timer WaveResetTimer;


	public override void _Ready()
	{
		WaveResetTimer = new Timer();
		WaveResetTimer.WaitTime = 3;
		WaveResetTimer.OneShot = true;
		WaveResetTimer.Timeout+=StartWave;
		AddChild(WaveResetTimer);
		WaveResetTimer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(CurrentWaveEnemy==0)
		{
			CurrentWaveMaxPoints = 10 + 1;
			CurrentWaveMaxEnemy = 1 + CurrentWave * 2;
			CurrentWaveEnemy = CurrentWaveMaxEnemy;
			CurrentWavePoints = 0;
			Lib.Print($"Starting wave {CurrentWave} with {CurrentWaveMaxEnemy} enemies");
			WaveResetTimer.WaitTime = Lib.rand.Next(1, 5);
			WaveResetTimer.Start();
		}
	}

	public void StartWave()
	{
		CurrentWave++;
		CurrentWaveEnemy = 0;
		while(CurrentWavePoints < CurrentWaveMaxPoints)
		{
			int enemyIndex = Lib.rand.Next(0, EnemyWindows.Length);
			int enemyWeight = EnimyWindowsWeights[enemyIndex];
			if(CurrentWaveEnemy<CurrentWaveMaxEnemy)
			{
				CurrentWavePoints += enemyWeight;
				CurrentWaveEnemy++;
				Window enemyWindow = EnemyWindows[enemyIndex].Instantiate<Window>();
				SpawnNode.AddChild(enemyWindow);
				
			}
			else
			{
				Lib.Print($"Enemy weight {enemyWeight} is too high for current wave {CurrentWave}");
			}
		}
	}

	public void EnemyDefeated()
	{
		CurrentWaveEnemy--;
	}
}
