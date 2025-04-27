using Godot;

public partial class MoveWindow : PopUpWindow
{
    [Export] private Sprite2D Jet;
    [Export] private Sprite2D Trail;
    double JitterTimer;

    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Size = Lib.GetAspectFactor(new Vector2I(400, 400));
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

        JitterTimer += delta;
        if (JitterTimer >= 0.1)
        {
            Jet.Position = Jet.Position + new Vector2I(Lib.rand.Next(-1, 2), Lib.rand.Next(-1, 2));
            Trail.Position = Trail.Position + new Vector2I(Lib.rand.Next(-1, 2), Lib.rand.Next(-1, 2));
            JitterTimer = 0;
        }
    }

    public override void TransitionFinished()
    {
        base.TransitionFinished();
        StartNewMovement();
    }

    private float CalculateMovementSpeed()
    {
        if (Level1.WindowCount >= 10)
            return Lib.rand.Next(80, 150) / 10f;
        else if (Level1.WindowCount >= 5)
            return Lib.rand.Next(30, 50) / 10f;
        else
            return Lib.rand.Next(15, 30) / 10f;
    }

    public void StartNewMovement()
    {
        Vector2I target = Lib.GetScreenPosition(Lib.GetRandomNormal(0.2f, 0.80f), Lib.GetRandomNormal(0.2f, 0.80f));
        float speed = CalculateMovementSpeed();
        StartExponentialTransition(target, speed, reset: true);

        Vector2I direction = target - Position;
        float angle = Mathf.RadToDeg(Mathf.Atan2(direction.Y, direction.X));
        Jet.RotationDegrees = angle + 90;
        Trail.RotationDegrees = angle + 90;
    }
}