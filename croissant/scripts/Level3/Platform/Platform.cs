using Godot;
using System;

public partial class Platform : CharacterBody2D
{
    [Export] public FloatWindow window;
    public bool Pressed = false;
    public Vector2 MouseOffset;
    public Level3 level3;
    public RectangleShape2D shape;
    [Export] public CollisionShape2D collisionShape;

    [Export] public Vector2 BaseSpeeds = new Vector2(20f, 20f);
    public Vector2 MinSpeedsWhenBlocked = new Vector2(1f, 1f);
    public float SpeedReductionFactor = 0.75f;
    public Vector2 SpeedRecoveryAmounts = new Vector2(200f, 200f);

    private Vector2 CurrentAppliedSpeeds;

    [Export] public bool Freeze = true;
    [Export] public ColorRect Texture;
    public ShaderMaterial Shader;

    public Vector2I CachedTitleBarSize;
    public bool WindowValid;
    public const float CollisionThreshold = 0.1f;
    public const float CollisionDotThreshold = -0.01f;

    public override void _Ready()
    {
        shape = collisionShape.Shape as RectangleShape2D;

        WindowValid = window != null && IsInstanceValid(window);
        if (WindowValid)
        {
            CachedTitleBarSize = window.TitleBarSize;
            window.Size = (Vector2I)shape.Size - CachedTitleBarSize;
            window.Position = (Vector2I)GlobalPosition + CachedTitleBarSize;
            window.Title = "Platform";
        }

        level3 = Level3.Instance;
        if (level3 != null)
        {
            level3.MouseEvent += MouseEvent;
        }

        CurrentAppliedSpeeds = BaseSpeeds;
        VisibilityChanged += VisibilityChange;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Freeze) return;

        if (Pressed)
        {
            if (!Visible)
            {
                Pressed = false;
                return;
            }

            Vector2 targetGlobalPosition = (Vector2)Lib.GetCursorPosition() - MouseOffset;
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
            window.Position = (Vector2I)GlobalPosition + CachedTitleBarSize;
        }
    }

    public void VisibilityChange()
    {
        Pressed = false;
    }

    public virtual void MouseEvent(InputEventMouseButton mouseButtonEvent)
    {
        if (!WindowValid || !window.Visible) return;

        if (!Freeze && mouseButtonEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseButtonEvent.Pressed && MouseOnWindow())
            {
                Pressed = true;
                MouseOffset = (Vector2)Lib.GetCursorPosition() - GlobalPosition;
            }
            else if (!mouseButtonEvent.Pressed && Pressed)
            {
                Pressed = false;
            }
        }
    }

    public bool MouseOnWindow()
    {
        if (!WindowValid) return false;

        Vector2I mousePos = Lib.GetCursorPosition();
        Vector2I windowPos = window.Position;
        Vector2I windowSize = window.Size;
        int titleBarHeight = window.TitleBarHeight;

        return mousePos.X >= windowPos.X &&
               mousePos.X <= windowPos.X + windowSize.X &&
               mousePos.Y >= windowPos.Y - titleBarHeight &&
               mousePos.Y < windowPos.Y;
    }

    public override void _ExitTree()
    {
        if (level3 != null)
        {
            level3.MouseEvent -= MouseEvent;
        }

        if (WindowValid)
        {
            window.QueueFree();
        }
        base._ExitTree();
    }
}
