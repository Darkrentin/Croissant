using Godot;
using System;

public partial class BulletHit : GpuParticles3D
{
	public override void _Ready()
	{
	}

	public override void _Process(double delta)
	{
	}

	void on_timer_timeout()
	{
		QueueFree();
	}
}
