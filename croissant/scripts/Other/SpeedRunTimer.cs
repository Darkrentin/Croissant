using Godot;
using System;

public partial class SpeedRunTimer : CanvasLayer
{
	[Export] public Label timer;
	[Export] public AnimationPlayer animationPlayer;
	public int Level = 10;
	public double Time = 0;

	public override void _Process(double delta)
	{
		Time += delta;
		if (Time > Level)
		{
			animationPlayer.Play("LevelReach");
			Level += 10;
		}
		timer.Text = FormatTime(Time);
	}

	private string FormatTime(double timeInSeconds)
	{
		int minutes = (int)(timeInSeconds / 60);
		int seconds = (int)(timeInSeconds % 60);
		int milliseconds = (int)((timeInSeconds - (minutes * 60) - seconds) * 1000);

		return string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
	}
}
