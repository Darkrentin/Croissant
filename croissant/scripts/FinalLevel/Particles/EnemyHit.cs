using Godot;

public partial class EnemyHit : GpuParticles3D
{
	public override void _Ready()
	{
		Emitting = true;
	}

	public void _on_timer_timeout()
	{
		QueueFree();
	}
}
