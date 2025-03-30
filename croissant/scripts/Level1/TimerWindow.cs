using Godot;
using System;

public partial class TimerWindow : PopUpWindow
{
    [Export] public Timer timer;
    [Export] public ProgressBar progressBar;

    private float time;

    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        time = CalculateTimerDuration();
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        timer.Timeout += _on_timer_timeout;
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 600));
        progressBar.Size = Size;
        timer.Start();
    }

        // more there is windows easier is it to close them
    private float CalculateTimerDuration()
    {
        if (Level1.WindowCount >= 10)
        {
            return Lib.rand.Next(9, 11);
        }
        else if (Level1.WindowCount >= 5)
        {
            return Lib.rand.Next(5, 7);
        }
        else
        {
            return Lib.rand.Next(3, 5);
        }
    }

    public override void OnClose()
    {
        Level1.WindowKill();
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        progressBar.Value = timer.TimeLeft * 100f;
    }

    public void _on_timer_timeout()
    {
        Level1.WindowCount--;
        for (int i = 0; i < 2; i++)
        {
            Level1.AddNewWindow();
        }
        QueueFree();
    }

}
