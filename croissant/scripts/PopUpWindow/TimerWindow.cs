using Godot;
using System;

public partial class TimerWindow : PopUpWindow
{
    [Export] public Timer timer;
    [Export] public ProgressBar progressBar;
    private Timer TitleTimer = new Timer();

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

    public override void _Ready()
    {
        base._Ready();

        AddChild(TitleTimer);
        TitleTimer.Timeout += ChangeTitle;
        ChangeTitle();

        Parent = GetParent<Level1>();
        time = CalculateTimerDuration();
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 600));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
        progressBar.Size = Size;
        timer.Start();
    }

    public override void OnClose()
    {
        Parent.WindowKillCount++;
        Parent.WindowCount--;
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        progressBar.Value = timer.TimeLeft * 100f;
    }

    public void _on_timer_timeout()
    {
        Parent.WindowCount--;
        for (int i = 0; i < 2; i++)
        {
            Parent.AddNewWindow();
        }
        QueueFree();
    }

    public void ChangeTitle()
    {
        Title = Lib.GetCursedString();
        TitleTimer.WaitTime = Lib.GetRandomNormal(0f, 0.5f);
        TitleTimer.Start();
    }
}
