using Godot;
using System;

public partial class MoveWindow : PopUpWindow
{
    private Timer TitleTimer = new Timer();
    public override void _Ready()
    {
        base._Ready();

        AddChild(TitleTimer);
        TitleTimer.Timeout += ChangeTitle;
        ChangeTitle();

        Parent = GetParent<Level1>(); ;
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(384, 264));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
        StartNewMovement();
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
    }

    public override void TransitionFinished()
    {
        base.TransitionFinished();
        StartNewMovement();
    }

    private float CalculateMovementSpeed()
    {
        if (Parent.WindowCount >= 10)
        {
            return Lib.rand.Next(80, 150) / 10f;
        }
        else if (Parent.WindowCount >= 5)
        {
            return Lib.rand.Next(30, 50) / 10f;
        }
        else
        {
            return Lib.rand.Next(15, 30) / 10f;
        }
    }

    public void StartNewMovement()
    {
        Vector2I target = Lib.GetScreenPosition(Lib.GetRandomNormal(0.2f, 0.80f), Lib.GetRandomNormal(0.2f, 0.80f));
        float speed = CalculateMovementSpeed();
        StartExponentialTransition(target, speed, reset: true);
    }

    public void ChangeTitle()
    {
        Title = Lib.GetCursedString();
        TitleTimer.WaitTime = Lib.GetRandomNormal(0f, 0.5f);
        TitleTimer.Start();
    }
}
