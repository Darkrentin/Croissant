using Godot;
using System;
using System.Numerics;

public partial class BombWindow : PopUpWindow
{
	[Export] public Timer timer;
	[Export] public ProgressBar progressBar;
	[Export] AnimatedSprite2D Sprite;

	private float time;
	public override void _Ready()
	{
		int randNum = Lib.rand.Next(0, 9);
		if (randNum == 8)
		{
			Sprite.Visible = true;
		}
		base._Ready();
		Parent = GetParent<Level1>();
		time = Lib.rand.Next(8, 10);
		progressBar.MaxValue = time * 100f;
		timer.WaitTime = time;
		Size = Lib.GetScreenSize(Lib.GetPercentage(new Vector2I(250, 250)));
		SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
		progressBar.Size = Size;
		timer.Start();

		Title = "💣";
	}

	public override void OnClose()
	{
		Parent.WindowCount--;
		for (int i = 0; i < 3; i++)
		{
			Parent.AddNewWindow();
		}
		QueueFree();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		progressBar.Value = timer.TimeLeft * 100f;
	}

	public void _on_timer_timeout()
	{
		Parent.WindowCount--;
		QueueFree();
	}

}
