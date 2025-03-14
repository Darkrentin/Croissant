using Godot;
using System;

public partial class TimerWindow: PopUpWindow
{
	[Export] public Timer timer;
	[Export] public ProgressBar progressBar;


    // more there is windows easier is it to close them
	private float CalculateTimerDuration()
    {
        if (Parent.WindowCount >= 10)
        {
            return Lib.rand.Next(9, 11); 
        }
        else if (Parent.WindowCount >= 5)
        {
            return Lib.rand.Next(5, 7); 
        }
        else
        {
            return Lib.rand.Next(3, 5); 
        }
    }

	private float time;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
        base._Ready();
        Parent = GetParent<Level1>();
        time = CalculateTimerDuration();
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        Size = new Vector2I(250,180);
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0.1f,0.9f),Lib.GetRandomNormal(0.1f,0.9f)));
        progressBar.Size = Size;
        timer.Start();
	}

	public override void OnClose()
	{
		Parent.WindowKillCount++;
		Parent.WindowCount--;
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
		Parent.WindowCount--;
        for (int i = 0; i < 2; i++)
        {
			Parent.OnSpawnTimerTimeout();
        }
		QueueFree();
    }

}
