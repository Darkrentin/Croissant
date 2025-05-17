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
    [Export] public ColorRect Texture;
    public ShaderMaterial Shader;

    public override void _Ready()
    {
        // First get the shape from collisionShape
        shape = collisionShape?.Shape as RectangleShape2D;
        if (shape == null)
        {
            //GD.PrintErr("No valid RectangleShape2D found on collisionShape");
            return;
        }

        // Then initialize the window if it exists
        if (window != null && IsInstanceValid(window))
        {
            //window.Visible = true;
            window.Size = (Vector2I)shape.Size - window.TitleBarSize;
            window.Position = (Vector2I)GlobalPosition + window.TitleBarSize;
        }

        // Get Level3 instance and subscribe to events
        level3 = Level3.Instance;
        if (level3 != null)
        {
            level3.MouseEvent += MouseEvent;
        }

        currentAppliedSpeeds = BaseSpeeds;
        window.Title = "Platform";
        Shader = Texture.Material as ShaderMaterial;
    }

    public override void _PhysicsProcess(double delta)
    {

        Shader.SetShaderParameter("window_size", window.Size);


        if (!Freeze)
        {
            if (Pressed)
            {
                if (!Visible)
                {
                    Pressed = false;
                    return;
                }
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
            window.Position = (Vector2I)GlobalPosition + window.TitleBarSize;
        }
        

        
    }

    public virtual void MouseEvent(InputEventMouseButton mouseButtonEvent)
    {
        if(!window.Visible)
            return;
        if (!IsInstanceValid(window) || window == null)
        {
            if (level3 != null)
            {
                level3.MouseEvent -= MouseEvent;
            }
            return;
        }

        try
        {
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
        catch (ObjectDisposedException)
        {
            if (level3 != null)
            {
                level3.MouseEvent -= MouseEvent;
            }
            Pressed = false;
        }

        
    }

    public bool MouseOnWindow()
    {
        // Check if window is valid before accessing it
        if (window == null || window.IsQueuedForDeletion())
        {
            return false;
        }

        try
        {
            Vector2I mousePos = Lib.GetCursorPosition();
            Vector2I windowPos = window.Position;
            Vector2I windowSize = window.Size;
            int titleBarHeight = window.TitleBarHeight;

            return mousePos.X >= windowPos.X &&
                   mousePos.X <= windowPos.X + windowSize.X &&
                   mousePos.Y >= windowPos.Y - titleBarHeight &&
                   mousePos.Y < windowPos.Y + windowSize.Y;
        }
        catch (ObjectDisposedException)
        {
            // Window has been disposed, return false
            return false;
        }
    }

    public override void _ExitTree()
    {
        if (level3 != null)
        {
            level3.MouseEvent -= MouseEvent;
        }

        if (IsInstanceValid(window))
        {
            window.QueueFree();
        }
        
        base._ExitTree();
    }
}
