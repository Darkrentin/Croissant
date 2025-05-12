using System;
using Godot;

public partial class WaveManager : Node
{
	[Export] public Node SpawnNode;
	[Export] public Wave FirstWave;
	
	[Export] public WaveData WaveData;
	[Export] public Label ScoreLabel;
	[Export] public AnimationPlayer AnimationPlayer;
	public int CurrentWaveId = 1;
	public Wave CurrentWave;
	public Wave LastWave;
	public Timer WaveStartTimer;

	public static int WaveNum = 0;
	public Action EndWave;


	public override void _Ready()
	{
		WaveStartTimer = new Timer();
		WaveStartTimer.WaitTime = 1f;
		WaveStartTimer.OneShot = true;
		WaveStartTimer.Timeout += StartWave;
		AddChild(WaveStartTimer);
		WaveStartTimer.Start();
	}

	public void StartWave()
	{
		FirstWave.StartWave();
	}

	public void GoBackToWave()
	{
		CurrentWaveId = LastWave.id;
		CurrentWave.WaveTimer.Stop();
		CleanUpAllWave(CurrentWave);
		WaveNum--;
		UpdateLabel();
		LastWave.StartWave();
		

	}

	public void AddWave()
	{
		WaveNum++;
		UpdateLabel();
		
	}

	public void CleanUpWave(Wave w)
	{
		w.CurrentWave = false;
		w.WaveWindows.Clear();
	}

	public void CleanUpAllWave(Wave endWave)
	{
		Wave current = LastWave.NextWave;
		while (current != endWave.NextWave)
		{
			CleanUpWave(current);
			current = current.NextWave;
		}
		Wave.NbOfEnemies = 0;

		foreach (AttackWindow window in SpawnNode.GetChildren())
		{
			window.Delete();
			SpawnNode.RemoveChild(window);
			window.QueueFree();
		}
	}

	public void UpdateLabel()
	{
		ScoreLabel.Text = WaveNum.ToString();
		AnimationPlayer.Play("ScoreUp");
	}

}