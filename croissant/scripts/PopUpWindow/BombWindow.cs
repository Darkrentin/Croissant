using Godot;
using System;

public partial class BombWindow: PopUpWindow
{
	[Export] public Timer timer;
	[Export] public ProgressBar progressBar;


	private float time;
	public override void _Ready()
	{
        base._Ready();
        Parent = GetParent<Level1>();
        time = Lib.rand.Next(9, 11); 
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        Size = new Vector2I(320,256);
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f,0.90f),Lib.GetRandomNormal(0f,0.90f)));
        progressBar.Size = Size;
        timer.Start();
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
	}


   public void _on_timer_timeout()
    {
		Parent.WindowKillCount++;
		Parent.WindowCount--;
		QueueFree();       
    }

}
