using Godot;
using System;

public partial class DodgeWindow : PopUpWindow
{
    private bool isMoving = false;
    private Timer cooldownTimer;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Parent = GetParent<Level1>();
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(390, 450));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
        cooldownTimer = new Timer();
        AddChild(cooldownTimer);
        cooldownTimer.WaitTime = 0.39f;
        cooldownTimer.OneShot = true;
        cooldownTimer.Timeout += OnCooldownTimerTimeout;
    }

    private void CheckInitialMousePosition()
    {
        if (IsMouseOnCloseButton())
        {
            StartNewMovement();
        }
    }

    public override void OnClose()
    {
        Parent.WindowKillCount++;
        Parent.WindowCount--;
        QueueFree();
    }

    private bool IsMouseOnCloseButton()
    {
        Vector2 localMousePos = Lib.GetCursorPosition() - Position;
        float margin = 20;

        float leftBound = -margin;
        float rightBound = Size.X + margin;
        float topBound = -margin;
        float bottomBound = Size.Y + margin;

        return localMousePos.X >= leftBound &&
                    localMousePos.X <= rightBound &&
                    localMousePos.Y >= topBound &&
                    localMousePos.Y <= bottomBound;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (IsMouseOnCloseButton() && !isMoving)
        {
            StartNewMovement();
            isMoving = true;
            cooldownTimer.Start();
        }
    }

    private void OnCooldownTimerTimeout()
    {
        isMoving = false;
    }

    private float CalculateMovementSpeed()
    {
        if (Parent.WindowCount >= 10)
        {
            return Lib.rand.Next(50, 80) / 10f;
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

}
