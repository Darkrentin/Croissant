using Godot;

public partial class MovePlatform : Platform
{
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
        if (!Visible)
            return;


        if (window != null && IsInstanceValid(window) && window.Visible)
        {
            bool mouseOver = MouseOnWindow();
            if (mouseOver)
                Shader.SetShaderParameter("speed", 0.3f);
            else
                Shader.SetShaderParameter("speed", 0.1f);
        }
        
        if((bool)Shader.GetShaderParameter("animate"))
        {
            float currentSpeed = (float)Shader.GetShaderParameter("speed");
            float timeIncrement = (float)delta * currentSpeed;
            float currentTime = (float)Shader.GetShaderParameter("Time") + timeIncrement;
            Shader.SetShaderParameter("Time", currentTime);
        }

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
