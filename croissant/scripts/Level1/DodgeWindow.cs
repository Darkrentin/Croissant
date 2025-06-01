using Godot;

public partial class DodgeWindow : PopUpWindow
{
    private bool isMoving = false;
    private Timer cooldownTimer;

    public override void _Ready()
    {
        HasChangingTitle = true;
        Size = Lib.GetAspectFactor(new Vector2I(390, 450));
        base._Ready();

        cooldownTimer = new Timer();
        AddChild(cooldownTimer);
        cooldownTimer.WaitTime = 0.39f;
        cooldownTimer.OneShot = true;
        cooldownTimer.Timeout += OnCooldownTimerTimeout;
    }

    private void CheckInitialMousePosition()
    {
        if (IsMouseOnCloseButton())
            StartNewMovement();
    }

    public override void OnClose()
    {
        GameManager.ClickSound.Play();
        Level1.WindowKill();
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
        return Lib.rand.Next(80, 100) / 10f;
    }

    public void StartNewMovement()
    {
        Vector2I target = Lib.GetScreenPosition(Lib.GetRandomNormal(0.2f, 0.80f), Lib.GetRandomNormal(0.2f, 0.80f));
        float speed = CalculateMovementSpeed();
        StartExponentialTransition(target, speed, reset: true);
    }
}
