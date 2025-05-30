using Godot;

public partial class MovePlatform : Platform
{
    [Export] public float MinMult = 1;
    [Export] public float MaxMult = 3;
    [Export] public float OscillationTime = 0.8f;
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
    private const float FastSpeed = 6f;
    private const float SlowSpeed = 0.5f;

    // Ajout pour le système de son de mouvement
    private Vector2 _lastSoundPosition;
    private float _totalDistanceTraveled = 0f;
    private const float SOUND_DISTANCE_THRESHOLD = 800f;

    public override void _Ready()
    {
        base._Ready();
        ShaderRectShader = ShaderRect.Material as ShaderMaterial;
        Shader = Texture.Material as ShaderMaterial;

        Shader.SetShaderParameter("window_size", window.Size);
        ShaderRectShader.SetShaderParameter("mult", MinMult);
        Freeze = false;

        // Initialiser la position de départ pour le calcul de distance
        _lastSoundPosition = GlobalPosition;

        // Configurer le son de mouvement pour éviter les coupures
        MoveSound.MaxPolyphony = 4;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 previousPosition = GlobalPosition;

        base._PhysicsProcess(delta);
        if (!Visible) return;

        // Calculer la distance parcourue depuis la dernière frame
        if (Pressed)
        {
            Vector2 currentPosition = GlobalPosition;
            float distanceThisFrame = _lastSoundPosition.DistanceTo(currentPosition);
            _totalDistanceTraveled += distanceThisFrame;

            // Jouer le son tous les 100 pixels
            if (_totalDistanceTraveled >= SOUND_DISTANCE_THRESHOLD)
            {
                MoveSound.Play();
                _totalDistanceTraveled = 0f;
                _lastSoundPosition = currentPosition;
            }
        }
        else
        {
            // Reset quand on ne bouge plus
            _lastSoundPosition = GlobalPosition;
            _totalDistanceTraveled = 0f;
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
                if (!Pressed)
                {
                    JustPressed();
                }
                Pressed = true;
                MouseOffset = (((Vector2)Lib.GetCursorPosition()) / Lib.GetScreenRatio()) - GlobalPosition;
                Shader.SetShaderParameter("frequency", 0.1f);
            }
            else if (!mouseButtonEvent.Pressed && Pressed)
            {
                JustReleased();
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

    public void JustPressed()
    {
        PressedSound.Play();
        _lastSoundPosition = GlobalPosition;
        _totalDistanceTraveled = 0f;
    }

    public void JustReleased()
    {
        ReleaseSound.Play();
        _totalDistanceTraveled = 0f;
    }

    public override void VisibilityChange()
    {
        base.VisibilityChange();
        //ShaderRectShader.SetShaderParameter("mult", MaxMult);
        //Shader.SetShaderParameter("frequency", 32f);
    }
}
