using Godot;
using System;

public partial class BulletHit : GpuParticles3D
{
	void on_timer_timeout()
	{
		QueueFree();
	}
}
