using Godot;
using System;

public partial class ParticulePreLoader : Node
{
	// Called when the node enters the scene tree for the first time.
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

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
