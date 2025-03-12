using Godot;
using System;

public partial class TimerWindow: PopUpWindow
{
	[Export] public Timer timer;
	[Export] public ProgressBar progressBar;

	private float time;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		time = Lib.rand.Next(2, 5);
		progressBar.MaxValue = time*100f;
		timer.WaitTime = time;
		Parent = GetParent<Level1>();
		Parent.WindowCount++;
		Size = new Vector2I(250,180);
		SetWindowPosition(GameManager.GetScreenPosition(Lib.GetRandomNormal(0.1f,0.9f),Lib.GetRandomNormal(0.1f,0.9f)));
		progressBar.Size = Size;
		timer.Start();
	}

	public override void OnClose()
	{
		Parent.WindowKillCount++;
		Parent.WindowCount--;
		GD.Print("cc");
		QueueFree();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		Update();
	}

    protected override void Update()
    {
        progressBar.Value = timer.TimeLeft*100f;
    }

	public void _on_timer_timeout()
	{
		GD.Print("Timer timeout");
		QueueFree();
		
	}

}
