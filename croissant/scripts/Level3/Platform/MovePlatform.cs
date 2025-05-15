using Godot;

public partial class MovePlatform : Platform
{
    public override void _Ready()
    {
        base._Ready();
        Freeze = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if(!Visible)
            return;
        

        if (window != null && IsInstanceValid(window) && window.Visible)
        {
            bool mouseOver = MouseOnWindow();
            if(mouseOver)
                Shader.SetShaderParameter("speed", 0.3f);
            else
                Shader.SetShaderParameter("speed", 0.1f);
        }

    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
