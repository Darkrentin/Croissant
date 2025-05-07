using Godot;
using System;
using System.Security.Principal;

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

    private Vector2 currentAppliedSpeeds;

    [Export] public bool Freeze = true;

    public override void _Ready()
    {
        window.Visible = true;
        Lib.Print("titlebar height: " + window.TitleBarHeight);
        level3 = (Level3)GetParent();
        level3.MouseEvent+=MouseEvent;

        shape = collisionShape.Shape as RectangleShape2D;

        shape.Size = (Vector2I)shape.Size;
        currentAppliedSpeeds = BaseSpeeds;
        window.Size = (Vector2I)shape.Size;
        window.Position = (Vector2I)GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        if(!Freeze)
        {
            if (Pressed)
            {
                Vector2 targetGlobalPosition = (Vector2)Lib.GetCursorPosition() - MouseOffset;
                Vector2 directionToTarget = targetGlobalPosition - GlobalPosition;

                Velocity = new Vector2(directionToTarget.X * currentAppliedSpeeds.X,
                                    directionToTarget.Y * currentAppliedSpeeds.Y);

                MoveAndSlide();

                var collision = GetLastSlideCollision();

                bool reduceX = false;
                bool reduceY = false;

                if (collision != null)
                {
                    Vector2 collisionNormal = collision.GetNormal();

                    if (Mathf.Abs(collisionNormal.X) > 0.1f)
                    {
                        if ((directionToTarget.X * currentAppliedSpeeds.X * collisionNormal.X) < -0.01f)
                        {
                            reduceX = true;
                        }
                    }

                    if (Mathf.Abs(collisionNormal.Y) > 0.1f)
                    {
                        if ((directionToTarget.Y * currentAppliedSpeeds.Y * collisionNormal.Y) < -0.01f)
                        {
                            reduceY = true;
                        }
                    }
                }

                if (reduceX)
                {
                    currentAppliedSpeeds.X = Mathf.Max(MinSpeedsWhenBlocked.X, currentAppliedSpeeds.X * SpeedReductionFactor);
                }
                else
                {
                    currentAppliedSpeeds.X = Mathf.Min(BaseSpeeds.X, currentAppliedSpeeds.X + SpeedRecoveryAmounts.X * (float)delta);
                }

                if (reduceY)
                {
                    currentAppliedSpeeds.Y = Mathf.Max(MinSpeedsWhenBlocked.Y, currentAppliedSpeeds.Y * SpeedReductionFactor);
                }
                else
                {
                    currentAppliedSpeeds.Y = Mathf.Min(BaseSpeeds.Y, currentAppliedSpeeds.Y + SpeedRecoveryAmounts.Y * (float)delta);
                }
            }
            else
            {
                Velocity = Vector2.Zero;
                currentAppliedSpeeds.X = Mathf.Min(BaseSpeeds.X, currentAppliedSpeeds.X + SpeedRecoveryAmounts.X * (float)delta);
                currentAppliedSpeeds.Y = Mathf.Min(BaseSpeeds.Y, currentAppliedSpeeds.Y + SpeedRecoveryAmounts.Y * (float)delta);
            }
            window.Position = (Vector2I)GlobalPosition;
        }
    }

    public void MouseEvent(InputEventMouseButton mouseButtonEvent)
    {
        Lib.Print("MouseEvent: " + mouseButtonEvent.Pressed);
        if (!Freeze && mouseButtonEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseButtonEvent.Pressed && MouseOnWindow())
            {
                GD.Print("Clic gauche ENFONCÉ dans la zone !");
                Pressed = true;
                MouseOffset = (Vector2)Lib.GetCursorPosition() - GlobalPosition;
            }
            else if (!mouseButtonEvent.Pressed && Pressed)
            {
                GD.Print("Clic gauche RELÂCHÉ !");
                Pressed = false;
            }
        }
    }

    public bool MouseOnWindow()
    {
        Vector2I mousePos = Lib.GetCursorPosition();
        Vector2I windowPos = window.Position;
        Vector2I windowSize = window.Size;
        int titleBarHeight = window.TitleBarHeight;

        return mousePos.X >= windowPos.X &&
               mousePos.X <= windowPos.X + windowSize.X &&
               mousePos.Y >= windowPos.Y - titleBarHeight &&
               mousePos.Y < windowPos.Y;
    }
}
