using Godot;

public partial class Platform : CharacterBody2D
{
    [Export] public WindowPlatform window;
    [Export] public CollisionShape2D collisionShape;
    [Export] public Vector2 BaseSpeeds = new Vector2(20f, 20f);
    [Export] public bool Freeze = true;
    [Export] public ColorRect Texture;

    public bool Pressed = false;
    public Vector2 MouseOffset;
    public Level3 Level3Instance;
    public RectangleShape2D Shape;
    public ShaderMaterial Shader;
    public Vector2I CachedTitleBarSize;
    public bool WindowValid;

    private Vector2 MinSpeedsWhenBlocked = new Vector2(1f, 1f);
    private float SpeedReductionFactor = 0.75f;
    private Vector2 SpeedRecoveryAmounts = new Vector2(200f, 200f);
    private Vector2 CurrentAppliedSpeeds;
    private const float CollisionThreshold = 0.1f;
    private const float CollisionDotThreshold = -0.01f;

    public override void _Ready()
    {
        Shape = collisionShape.Shape as RectangleShape2D;
        WindowValid = IsInstanceValid(window);

        if (WindowValid)
        {
            CachedTitleBarSize = window.TitleBarSize;
            window.Size = (Vector2I)((Shape.Size - CachedTitleBarSize) * Lib.GetScreenRatio());
            window.Position = (Vector2I)GlobalPosition + CachedTitleBarSize;

            // Add platform window to global list
            if (!GameManager.Windows.Contains(window))
            {
                GameManager.Windows.Add(window);
            }

            // Set up deletion callback for platform windows
            window.DeleteWindow = () =>
            {
                GameManager.Windows.Remove(window);
            };
        }

        Level3Instance = Level3.Instance;
        CurrentAppliedSpeeds = BaseSpeeds;
        VisibilityChanged += VisibilityChange;
        window.Title = "";
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Pressed)
        {
            if (!Visible)
            {
                Pressed = false;
                return;
            }

            Vector2 targetGlobalPosition = ((Vector2)(Lib.GetCursorPosition()) / Lib.GetScreenRatio()) - MouseOffset;
            Vector2 directionToTarget = targetGlobalPosition - GlobalPosition;

            Velocity = directionToTarget * CurrentAppliedSpeeds;
            MoveAndSlide();

            var collision = GetLastSlideCollision();
            bool reduceX = false;
            bool reduceY = false;

            if (collision != null)
            {
                Vector2 collisionNormal = collision.GetNormal();
                float absNormalX = Mathf.Abs(collisionNormal.X);
                float absNormalY = Mathf.Abs(collisionNormal.Y);

                if (absNormalX > CollisionThreshold)
                {
                    reduceX = (directionToTarget.X * CurrentAppliedSpeeds.X * collisionNormal.X) < CollisionDotThreshold;
                }

                if (absNormalY > CollisionThreshold)
                {
                    reduceY = (directionToTarget.Y * CurrentAppliedSpeeds.Y * collisionNormal.Y) < CollisionDotThreshold;
                }
            }

            float deltaFloat = (float)delta;

            CurrentAppliedSpeeds.X = reduceX
                ? Mathf.Max(MinSpeedsWhenBlocked.X, CurrentAppliedSpeeds.X * SpeedReductionFactor)
                : Mathf.Min(BaseSpeeds.X, CurrentAppliedSpeeds.X + SpeedRecoveryAmounts.X * deltaFloat);

            CurrentAppliedSpeeds.Y = reduceY
                ? Mathf.Max(MinSpeedsWhenBlocked.Y, CurrentAppliedSpeeds.Y * SpeedReductionFactor)
                : Mathf.Min(BaseSpeeds.Y, CurrentAppliedSpeeds.Y + SpeedRecoveryAmounts.Y * deltaFloat);
        }
        else
        {
            Velocity = Vector2.Zero;
            float deltaFloat = (float)delta;
            CurrentAppliedSpeeds.X = Mathf.Min(BaseSpeeds.X, CurrentAppliedSpeeds.X + SpeedRecoveryAmounts.X * deltaFloat);
            CurrentAppliedSpeeds.Y = Mathf.Min(BaseSpeeds.Y, CurrentAppliedSpeeds.Y + SpeedRecoveryAmounts.Y * deltaFloat);
        }

        if (WindowValid)
        {
            window.Position = (Vector2I)((GlobalPosition + CachedTitleBarSize) * Lib.GetScreenRatio());
        }
        base._PhysicsProcess(delta);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void VisibilityChange()
    {
        Pressed = false;
    }

    public override void _ExitTree()
    {
        if (WindowValid)
        {
            // Remove from global list before freeing
            GameManager.Windows.Remove(window);
            window.QueueFree();
        }
        base._ExitTree();
    }
}
