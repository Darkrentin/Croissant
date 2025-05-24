using Godot;

public partial class MovePlatform : Platform
{
    private float CurrentTime;
    private float CurrentSpeed = 0.1f;
    private const float FastSpeed = 0.3f;
    private const float SlowSpeed = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        Shader = Texture.Material as ShaderMaterial;
        Shader.SetShaderParameter("window_size", window.Size);
        Freeze = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (!Visible) return;

        if (WindowValid && window.Visible)
        {
            bool mouseOver = MouseOnWindow();
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
}
