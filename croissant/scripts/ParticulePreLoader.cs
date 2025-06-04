using Godot;
using System;

public partial class ParticulePreLoader : Node
{
	[Export] public CpuParticles2D[] Particles2D;
	[Export] public GpuParticles3D[] Particles3D;
	public override void _Ready()
	{
		foreach (var particle in Particles2D)
		{
			particle.Emitting = true;
		}

		foreach (var particle in Particles3D)
		{
			particle.Emitting = true;
		}
	}
}
