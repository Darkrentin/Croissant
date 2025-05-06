using System;
using Godot;

public partial class WaveManager : Node
{
	[Export] public Node SpawnNode;
	public int CurrentWave = 1;
	[Export] public Wave FirstWave;
	[Export] public WaveData WaveData;
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

	public override void _Process(double delta)
	{

	}

	public void StartWave()
	{
		FirstWave.StartWave();
	}


}