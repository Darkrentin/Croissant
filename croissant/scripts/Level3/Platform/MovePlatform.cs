using Godot;

public partial class MovePlatform : Platform
{
    [Export] public float MinMult = 1;
    [Export] public float MaxMult = 3;
    [Export] public float OscillationTime = 0.8f;
    [Export] public ColorRect ShaderRect;
    public ShaderMaterial ShaderRectShader;
    private float LerpTimer = 0.0f;
    private bool IsLerping = false;
    private bool LerpDirection = true;
    private bool CurrentlyActive = false;
    private float CurrentTime;
    private float CurrentSpeed = 0.5f;
    private const float FastSpeed = 6f;
    private const float SlowSpeed = 0.5f;

    public override void _Ready()
    {
        base._Ready();
        ShaderRectShader = ShaderRect.Material as ShaderMaterial;
        Shader = Texture.Material as ShaderMaterial;

        Shader.SetShaderParameter("window_size", window.Size);
        ShaderRectShader.SetShaderParameter("mult", MinMult);
        Freeze = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!Visible) return;

        if (WindowValid && window.Visible)
        {
            bool mouseOnTitle = MouseOnTitle();
            bool shouldBeActive = mouseOnTitle || Pressed;

            if (shouldBeActive && !CurrentlyActive && !IsLerping)
            {
                IsLerping = true;
                LerpDirection = true;
                LerpTimer = 0.0f;
                CurrentlyActive = true;
            }
            else if (!shouldBeActive && CurrentlyActive && !IsLerping)
            {
                IsLerping = true;
                LerpDirection = false;
                LerpTimer = 0.0f;
                CurrentlyActive = false;
            }

            if (IsLerping)
            {
                LerpTimer += (float)delta;
                float lerpFactor = Mathf.Clamp(LerpTimer / OscillationTime, 0.0f, 1.0f);

                if (LerpDirection)
                {
                    int currentMult = Mathf.RoundToInt(Mathf.Lerp(MinMult, MaxMult, lerpFactor));
                    ShaderRectShader.SetShaderParameter("mult", currentMult);

                    if (lerpFactor >= 1.0f)
                    {
                        IsLerping = false;
                        ShaderRectShader.SetShaderParameter("mult", MaxMult);
                    }
                }
                else
                {
                    int currentMult = Mathf.RoundToInt(Mathf.Lerp(MaxMult, MinMult, lerpFactor));
                    ShaderRectShader.SetShaderParameter("mult", currentMult);

                    if (lerpFactor >= 1.0f)
                    {
                        IsLerping = false;
                        ShaderRectShader.SetShaderParameter("mult", MinMult);
                    }
                }
            }

            bool mouseOver = MouseOnTitle();
            float targetSpeed = (mouseOver || Pressed) ? FastSpeed : SlowSpeed;

            if (CurrentSpeed != targetSpeed)
            {
                CurrentSpeed = targetSpeed;
                Shader.SetShaderParameter("speed", CurrentSpeed);
            }
        }

        if ((bool)Shader.GetShaderParameter("animate"))
        {
            CurrentTime += (float)delta * CurrentSpeed;
            Shader.SetShaderParameter("Time", CurrentTime);
        }
    }

    public bool MouseOnTitle()
    {
        Vector2I mousePos = Lib.GetCursorPosition();
        Vector2I windowPos = window.Position;
        Vector2I windowSize = window.Size;
        int titleBarHeight = window.TitleBarHeight;

        return mousePos.X >= windowPos.X &&
               mousePos.X <= windowPos.X + windowSize.X &&
               mousePos.Y >= windowPos.Y - titleBarHeight &&
               mousePos.Y <= windowPos.Y;
    }

    public virtual void MouseEvent(InputEventMouseButton mouseButtonEvent)
    {
        if (!WindowValid || !window.Visible) return;

        if (!Freeze && mouseButtonEvent.ButtonIndex == MouseButton.Left)
        {
            if (mouseButtonEvent.Pressed && MouseOnTitle())
            {
                Pressed = true;
                MouseOffset = (Vector2)Lib.GetCursorPosition() - GlobalPosition;
                Shader.SetShaderParameter("frequency", 0.1f);
            }
            else if (!mouseButtonEvent.Pressed && Pressed)
            {
                Pressed = false;
                Shader.SetShaderParameter("frequency", 32f);
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
               mousePos.Y <= windowPos.Y + windowSize.Y;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            MouseEvent(mouseButtonEvent);
        }
    }
}
