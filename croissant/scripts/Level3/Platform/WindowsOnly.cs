using Godot;

public partial class WindowsOnly : Platform
{
    [Export] public ColorRect HighlightRect;
    private ShaderMaterial highlightShader;
    private Shader platformHighlightShader;
    private Shader plainHighlightShader;

    public override void _Ready()
    {
        base._Ready();
        Shader = Texture.Material as ShaderMaterial;
        Shader.SetShaderParameter("window_size", window.Size);

        highlightShader = HighlightRect.Material as ShaderMaterial;
        platformHighlightShader = GD.Load<Shader>("res://assets/shaders/PlatformHighlight.gdshader");
        plainHighlightShader = GD.Load<Shader>("res://assets/shaders/PlainHighlight.gdshader");
        Shader.SetShaderParameter("color1", Colors.Yellow);
        Shader.SetShaderParameter("frequency", 32f);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (Level3.Instance.MovingHovered)
        {
            highlightShader.Shader = platformHighlightShader;
        }
        else
        {
            highlightShader.Shader = plainHighlightShader;
        }
    }
}
