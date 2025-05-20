using Godot;

public partial class KillingPlatform : Platform
{
    [Export] public Area2D area2D;
    [Export] public CpuParticles2D cpuParticles2D_Black;
    [Export] public CpuParticles2D cpuParticles2D_R;

    public override void _Ready()
    {
        base._Ready();
        area2D.BodyEntered += OnBodyEntered;
        VisibilityChanged += VisibiltyChanged;
        /*
        Shader.SetShaderParameter("viewport_size", window.Size);
        Shader.SetShaderParameter("seed", Lib.GetRandomNormal(0.0f, 100.0f));
        Shader.SetShaderParameter("num_impacts", Lib.rand.Next(1, 6));
        Shader.SetShaderParameter("cracks_per_impact", 10);
        */
        cpuParticles2D_Black.Emitting = true;
        cpuParticles2D_Black.Position = (window.Size / 2) + window.TitleBarSize;
        if (cpuParticles2D_Black.EmissionShape is CpuParticles2D.EmissionShapeEnum.Rectangle)
        {
            cpuParticles2D_Black.EmissionRectExtents = (window.Size / 2) - window.TitleBarSize;
        }

        cpuParticles2D_R.Emitting = true;
        cpuParticles2D_R.Position = (window.Size / 2) + window.TitleBarSize;
        if (cpuParticles2D_R.EmissionShape is CpuParticles2D.EmissionShapeEnum.Rectangle)
        {
            cpuParticles2D_R.EmissionRectExtents = (window.Size / 2) - window.TitleBarSize;
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayerCharacter player)
        {
            player.Death();
        }
    }

    public void VisibiltyChanged()
    {
        if (Visible)
        {
            cpuParticles2D_Black.Emitting = true;
            cpuParticles2D_R.Emitting = true;
        }
        else
        {
            cpuParticles2D_Black.Emitting = false;
            cpuParticles2D_R.Emitting = false;
        }
    }
}