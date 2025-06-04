using Godot;
using System;

public partial class SpeedRunTimer : CanvasLayer
{
	[Export] public Label timer;
	[Export] public AnimationPlayer animationPlayer;
	public int Level = 60;
	public double Time = 0;
	public bool RecordTime = false;
	public static SpeedRunTimer Instance;
	public override void _Ready()
	{
		Instance = this;
		timer.Text = FormatTime(Time);
	}

	public override void _Process(double delta)
	{
		if (!RecordTime)
			return;
		Time += delta;
		if (Time > Level)
		{
			animationPlayer.Play("LevelReach");
			Level += 60;
		}
		timer.Text = FormatTime(Time);
	}

	private string FormatTime(double timeInSeconds)
	{
		int minutes = (int)(timeInSeconds / 60);
		int seconds = (int)(timeInSeconds % 60);
		int milliseconds = (int)((timeInSeconds - (minutes * 60) - seconds) * 1000);
		// Only take the first 2 digits of milliseconds by dividing by 10
		int twoDigitMilliseconds = milliseconds / 10;

		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, twoDigitMilliseconds);
	}

	public void StopTimer()
	{
		RecordTime = false;
		timer.Text = FormatTime(Time);
	}

	public void StartTimer()
	{
		RecordTime = true;
		Time = 0;
		timer.Text = FormatTime(Time);
	}
}
