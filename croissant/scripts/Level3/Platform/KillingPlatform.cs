using Godot;

public partial class KillingPlatform : Platform
{
    [Export] public Area2D area2D;
    [Export] public CpuParticles2D cpuParticles2D_Black;
    [Export] public CpuParticles2D cpuParticles2D_R;

    private Vector2 ParticlePosition;
    private Vector2 ParticleExtents;

    public override void _Ready()
    {
        base._Ready();
        area2D.BodyEntered += OnBodyEntered;
        VisibilityChanged += VisibiltyChanged;

        ParticlePosition = (window.Size / 2) + CachedTitleBarSize;
        ParticleExtents = (window.Size / 2) - CachedTitleBarSize;

        SetupParticles(cpuParticles2D_Black);
        SetupParticles(cpuParticles2D_R);
    }

    private void SetupParticles(CpuParticles2D particles)
    {
        particles.Emitting = true;
        particles.Position = ParticlePosition;
        if (particles.EmissionShape == CpuParticles2D.EmissionShapeEnum.Rectangle)
        {
            particles.EmissionRectExtents = ParticleExtents;
        }
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayerCharacter player && !player.isInvincible)
        {
            player.Death();
        }
    }

    public void VisibiltyChanged()
    {
        bool emit = Visible;
        cpuParticles2D_Black.Emitting = emit;
        cpuParticles2D_R.Emitting = emit;
    }
}
