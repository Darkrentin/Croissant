using Godot;

public partial class TimerWindow : PopUpWindow
{
    [Export] public Timer timer;
    [Export] public ProgressBar progressBar;
    private float time;

    public override void _Ready()
    {
        HasChangingTitle = true;
        Size = Lib.GetAspectFactor(new Vector2I(400, 600));
        base._Ready();

        time = CalculateTimerDuration();
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        timer.Timeout += _on_timer_timeout;

        progressBar.Size = Size;
        timer.Start();
    }

    // more there is windows easier is it to close them
    private float CalculateTimerDuration()
    {
        if (Level1.WindowCount >= 15)
            return Lib.rand.Next(8, 10);
        else if (Level1.WindowCount >= 10)
            return Lib.rand.Next(6, 8);
        else
            return Lib.rand.Next(4, 6);
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
        if (timer.TimeLeft <= 1 && timer.TimeLeft >= 0.9)
        {
            GrabFocus();
        }
    }

    public void _on_timer_timeout()
    {
        Level1.WindowKill();
        for (int i = 0; i < 2; i++)
            Level1.AddNewWindow();
        QueueFree();
    }
}