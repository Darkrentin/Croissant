using System;
using Godot;

public partial class WaveManager : Node
{
	[Export] public AudioStreamPlayer WaveBeginSound;
	[Export] public AudioStreamPlayer WavePrevious;
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
	public bool IsWaveGoBack = false;
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
	}

	public void StartWave()
	{
		FirstWave.StartWave();
	}

	public void OnAnimationFinished(StringName anim)
	{
		if (anim == "GoBack")
		{
			UpdateLabel((WaveNum + 1).ToString());
			AnimationPlayer.Play("GoBackReverse");
			WavePrevious.Play();
		}
		if (anim == "GoBackReverse")
		{
			LastWave.StartWave();
			Level2.CursorWindow.animationPlayer.PlayBackwards("Disolve");
			Level2.CursorWindow.Freeze = false;
		}
	}

	public void GoBackToWave()
	{
		CurrentWaveId = LastWave.id;
		CurrentWave.WaveTimer.Stop();
		CleanUpAllWave(CurrentWave);
		UpdateLabel(" ");
		WaveNum--;
		IsWaveGoBack = true;

		AnimationPlayer.Play("GoBack");
	}

	public void AddWave()
	{
		WaveNum++;
		UpdateLabel();
		if (IsWaveGoBack)
		{
			IsWaveGoBack = false;
		}
		else
		{
			WaveBeginSound.Play();
		}
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

	public void UpdateLabel(string text = "")
	{
		if (text != "")
		{
			ScoreLabel.Text = text;
			if (text != " ")
			{
				ScoreLabel.Text += "/4";
			}
		}
		else
		{
			ScoreLabel.Text = WaveNum.ToString() + "/4";
		}
		AnimationPlayer.Play("ScoreUp");
	}
}
