using System;
using Godot;

public partial class WaveManager : Node
{
	[Export] public Node SpawnNode;
	[Export] public Wave FirstWave;
	[Export] public WaveData WaveData;
	public int CurrentWave = 1;
	public Timer WaveStartTimer;
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
}