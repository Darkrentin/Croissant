using Godot;

public partial class WindowExplosion : CpuParticles2D
{
	[Export] private Timer timer;

	public override void _Ready()
	{
		Emitting = true;
		timer.Timeout += () =>
		{
			GetParent().RemoveChild(this);
			QueueFree();
		};
	}

	public override void _Process(double delta)
	{

	}
}
