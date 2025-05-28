using Godot;

public partial class ClickParticles : CpuParticles2D
{
	public void _on_timer_timeout()
	{
		QueueFree();
	}
}
