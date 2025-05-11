using Godot;

public partial class ObjectiveExplosion : GpuParticles3D
{
	[Export] public Timer Timer;
	public void _on_timer_timeout()
	{
		QueueFree();
	}
}