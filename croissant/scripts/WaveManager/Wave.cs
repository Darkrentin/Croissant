using Godot;
using System;
using System.Collections.Generic;

public partial class Wave : Node
{
	[Export] public bool WaitWave = false;
	[Export] public float duration = 10f;
	[Export] public int id = 0;
	[Export] public Wave NextWave;
	public WaveManager WaveManager;
	public List<FloatWindow> WaveWindows;
	public bool CurrentWave = false;
	public static int NbOfEnemies = 0;
	public Timer WaveTimer;
	public override void _Ready()
	{
		WaveManager = (WaveManager)GetParent();
		WaveTimer = new Timer();
		if (duration > 0)
		{
			WaveTimer.WaitTime = duration;
			WaveTimer.OneShot = true;
			WaveTimer.Timeout += StartNextWave;
			AddChild(WaveTimer);
		}
	}

	public void StartWave()
	{
		Lib.Print("Starting wave " + id);
		CurrentWave = true;
		if (!WaitWave)
		{
			WaveWindows = WaveManager.WaveData.WaveStart[id - 1]();
			foreach (FloatWindow window in WaveWindows)
			{
				window.DeleteWindow += EnemyDefeated;
				NbOfEnemies++;
				WaveManager.SpawnNode.AddChild(window);
			}
		}

		if (duration > 0)
		{
			WaveTimer.Start();
		}
	}

	public void StartNextWave()
	{
		WaveTimer.Stop();
		if (NextWave != null && CurrentWave)
		{
			NextWave.StartWave();
			CurrentWave = false;
		}
		else if (NextWave == null && CurrentWave)
		{
			WaveManager.EndWave();
			CurrentWave = false;
		}
	}

	public void EnemyDefeated()
	{
		NbOfEnemies--;
		if (NbOfEnemies <= 0 && !WaitWave)
		{
			StartNextWave();
		}
		else
		{
			GD.Print("Enemies left: " + NbOfEnemies);
		}
	}
}