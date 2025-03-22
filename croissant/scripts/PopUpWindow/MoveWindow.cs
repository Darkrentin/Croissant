using Godot;
using System;

public partial class MoveWindow : PopUpWindow
{
    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(384, 264));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
        StartNewMovement();
    }

    public override void OnClose()
    {
        Level1.WindowKill();
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
        if (Level1.WindowCount >= 10)
        {
            return Lib.rand.Next(80, 150) / 10f;
        }
        else if (Level1.WindowCount >= 5)
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

}