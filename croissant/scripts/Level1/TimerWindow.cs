using Godot;

public partial class TimerWindow : PopUpWindow
{
    [Export] public Timer timer;
    [Export] public ProgressBar progressBar;
    [Export] public Label timeLabel;
    private float time;

    public override void _Ready()
    {
        HasChangingTitle = true;
        Size = Lib.GetAspectFactor(new Vector2I(600, 400));
        base._Ready();

        time = CalculateTimerDuration();
        progressBar.MaxValue = time;
        timer.WaitTime = time;
        timer.Timeout += _on_timer_timeout;

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
        GameManager.ClickSound.Play();
        Level1.WindowKill();
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        progressBar.Value = time - timer.TimeLeft;
        timeLabel.Text = $"{Mathf.Ceil(timer.TimeLeft * 10) / 10:0.0}s";
        if (timer.TimeLeft <= 3f && timer.TimeLeft >= 2.9f)
            GrabFocus();
    }

    public void _on_timer_timeout()
    {
        Level1.WindowKill();
        for (int i = 0; i < 2; i++)
            Level1.AddNewWindow();
        QueueFree();
    }
}
