using Godot;

public partial class MovePlatform : Platform
{
    [Export] public float MinMult = 1.5f;
    [Export] public float MaxMult = 2.5f;
    [Export] public float OscillationTime = 0.4f;
    [Export] public ColorRect ShaderRect;
    [Export] public AudioStreamPlayer MoveSound;
    [Export] public AudioStreamPlayer PressedSound;
    [Export] public AudioStreamPlayer ReleaseSound;
    public ShaderMaterial ShaderRectShader;
    private float LerpTimer = 0.0f;
    private bool IsLerping = false;
    private bool LerpDirection = true;
    private bool CurrentlyActive = false;
    private float CurrentTime;
    private float CurrentSpeed = 0.5f;
    private const float FastSpeed = 20f;
    private const float SlowSpeed = 0.5f;

    private Shader platformShader;
    private Shader plainShader;

    private Vector2 _lastSoundPosition;
    private float _totalDistanceTraveled = 0f;
    private const float SOUND_DISTANCE_THRESHOLD = 700f;

    public bool Pressed = false;
    private Vector2 MinSpeedsWhenBlocked = new Vector2(1f, 1f);
    private float SpeedReductionFactor = 0.75f;
    private Vector2 SpeedRecoveryAmounts = new Vector2(200f, 200f);
    private Vector2 CurrentAppliedSpeeds;
    private const float CollisionThreshold = 0.1f;
    private const float CollisionDotThreshold = -0.01f;

    public override void _Ready()
    {
        base._Ready();
        ShaderRectShader = ShaderRect.Material as ShaderMaterial;
        Shader = Texture.Material as ShaderMaterial;

        platformShader = Shader.Shader;
        plainShader = GD.Load<Shader>("res://assets/shaders/PlainHighlight.gdshader");

        Shader.SetShaderParameter("window_size", window.Size);
        ShaderRectShader.SetShaderParameter("mult", MaxMult);
        Freeze = false;
        _lastSoundPosition = GlobalPosition;

        MoveSound.MaxPolyphony = 8;
        CurrentAppliedSpeeds = BaseSpeeds;
        VisibilityChanged += VisibilityChange;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!Visible) return;

        if (Pressed)
        {
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

            Vector2 currentPosition = GlobalPosition;
            float distanceThisFrame = _lastSoundPosition.DistanceTo(currentPosition);
            _totalDistanceTraveled += distanceThisFrame;

            if (_totalDistanceTraveled >= SOUND_DISTANCE_THRESHOLD)
            {
                MoveSound.Play();
                _totalDistanceTraveled = 0f;
                _lastSoundPosition = currentPosition;
            }
        }
        else
        {
            Velocity = Vector2.Zero;
            float deltaFloat = (float)delta;
            CurrentAppliedSpeeds.X = Mathf.Min(BaseSpeeds.X, CurrentAppliedSpeeds.X + SpeedRecoveryAmounts.X * deltaFloat);
            CurrentAppliedSpeeds.Y = Mathf.Min(BaseSpeeds.Y, CurrentAppliedSpeeds.Y + SpeedRecoveryAmounts.Y * deltaFloat);

            _lastSoundPosition = GlobalPosition;
            _totalDistanceTraveled = 0f;
        }

        if (WindowValid)
        {
            window.Position = (Vector2I)((GlobalPosition + CachedTitleBarSize) * Lib.GetScreenRatio());
        }

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
                float lerpFactor = Mathf.Clamp(LerpTimer / OscillationTime, 0.0f, 1f);

                if (LerpDirection)
                {
                    int currentMult = Mathf.RoundToInt(Mathf.Lerp(MinMult, MaxMult, lerpFactor));
                    ShaderRectShader.SetShaderParameter("mult", currentMult);

                    if (lerpFactor >= 1f)
                    {
                        IsLerping = false;
                        ShaderRectShader.SetShaderParameter("mult", MaxMult);
                    }
                }
                else
                {
                    int currentMult = Mathf.RoundToInt(Mathf.Lerp(MaxMult, MinMult, lerpFactor));
                    ShaderRectShader.SetShaderParameter("mult", currentMult);

                    if (lerpFactor >= 1f)
                    {
                        IsLerping = false;
                        ShaderRectShader.SetShaderParameter("mult", MinMult);
                    }
                }
            }
            else
            {
                ShaderRectShader.SetShaderParameter("mult", CurrentlyActive ? MaxMult : MinMult);
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

        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Pressed)
        {
            Shader.Shader = plainShader;
            Shader.SetShaderParameter("border_color", Colors.Cyan);
            Shader.SetShaderParameter("border_width", 4f);
        }
        else
        {
            Shader.Shader = platformShader;
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
                if (!Pressed)
                {
                    JustPressed();
                }
                Pressed = true;
                MouseOffset = (((Vector2)Lib.GetCursorPosition()) / Lib.GetScreenRatio()) - GlobalPosition;
            }
            else if (!mouseButtonEvent.Pressed && Pressed)
            {
                JustReleased();
                Pressed = false;
            }
        }
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            MouseEvent(mouseButtonEvent);
        }
    }

    public void JustPressed()
    {
        PressedSound.Play();
        _lastSoundPosition = GlobalPosition;
        _totalDistanceTraveled = 0f;
        Level3.Instance.NbPressedWindows++;
    }

    public void JustReleased()
    {
        ReleaseSound.Play();
        _totalDistanceTraveled = 0f;
        Level3.Instance.NbPressedWindows--;
    }

    public void VisibilityChange()
    {
        if (Pressed)
        {
            Level3.Instance.NbPressedWindows--;
            Pressed = false;
            Shader.Shader = platformShader;
        }

        ShaderRectShader.SetShaderParameter("mult", MaxMult);
        Shader.SetShaderParameter("frequency", 32f);
    }
}
