using System;
using Godot;

public partial class WaveManager : Node
{
	[Export] public AudioStreamPlayer WaveStartSound;
	[Export] public AudioStreamPlayer WaveTransitionSound;
	[Export] public AudioStreamPlayer WaveEndSound;
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
		AnimationPlayer.AnimationFinished += OnAnimationFinished;
		EndWave += () => WaveEndSound.Play();
	}

	public void StartWave()
	{
		WaveStartSound.Play();
		FirstWave.StartWave();
	}

	public void OnAnimationFinished(StringName anim)
	{
		if (anim == "GoBack")
		{
			UpdateLabel();
			AnimationPlayer.Play("GoBackReverse");
		}
		if (anim == "GoBackReverse")
		{
			LastWave.StartWave();
			Level2.CursorWindow.animationPlayer.PlayBackwards("Disolve");
		}
	}

	public void GoBackToWave()
	{
		CurrentWaveId = LastWave.id;
		CurrentWave.WaveTimer.Stop();
		CleanUpAllWave(CurrentWave);
		WaveNum--;
		Lib.Print($"WaveManager: GoBackToWave {WaveNum}");
		AnimationPlayer.Play("GoBack");
	}

	public void AddWave()
	{
		WaveNum++;
		UpdateLabel();
		Lib.Print($"WaveManager: AddWave {WaveNum}");
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
		foreach (AttackWindow window in SpawnNode.GetChildren())
		{
			window.Delete();
			SpawnNode.RemoveChild(window);
			window.QueueFree();
		}
		Wave.NbOfEnemies = 0;
	}

	public void UpdateLabel()
	{
		WaveTransitionSound.Play();
		ScoreLabel.Text = WaveNum.ToString();
		AnimationPlayer.Play("ScoreUp");
	}
}
