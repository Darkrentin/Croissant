using Godot;
using System;

public partial class SpeedRunTimer : CanvasLayer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Label timer;
	[Export] public AnimationPlayer animationPlayer;
	public int Level = 10;
	public double Time = 0;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Time+=delta;
		if(Time>Level)
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

        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }
}
