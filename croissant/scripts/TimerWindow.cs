using Godot;
using System;

public partial class TimerWindow: PopUpWindow
{
	[Export] public Timer timer;
	[Export] public ProgressBar progressBar;
    private int divisionCount = 0; // Compteur de divisions
    private const int MAX_DIVISIONS = 2; // Limite maximum de divisions


	private float time;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		time = Lib.rand.Next(3, 5);
		progressBar.MaxValue = time*100f;
		timer.WaitTime = time;
		Parent = GetParent<Level1>();
		Parent.WindowCount++;
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
		if (divisionCount != MAX_DIVISIONS)
		{
			progressBar.Value = timer.TimeLeft*100f;
		}
		
    }

   public void _on_timer_timeout()
    {
        // GD.Print("Timer timeout");
        if (divisionCount < MAX_DIVISIONS)
        {
			Parent.WindowCount--;
            for (int i = 0; i < 2; i++)
            {
                //GD.Print("Creating new window"); 
                TimerWindow newWindow = Parent.TimerWindowScene.Instantiate<TimerWindow>();
                Parent.AddChild(newWindow);
                newWindow.Size = new Vector2I((int)(Size.X * 0.7f), (int)(Size.Y * 0.7f));
                Vector2I offset = i == 0 ? new Vector2I(-50, -50) : new Vector2I(50, 50);
                newWindow.Position = Position + offset;
                newWindow.divisionCount = this.divisionCount + 1;
				
				QueueFree();
            }
        }
    }

}
