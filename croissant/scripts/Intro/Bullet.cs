using Godot;
using System;
using System.Linq;

public partial class Bullet : StaticBody2D
{
	PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Explosion.tscn");
	[Export] Vector2 Velocity = new Vector2(100.0f, 100.0f);

	public override void _Ready()
	{
		Velocity = -(GetParent().GetNode<Player>("Player").GlobalPosition - GetGlobalMousePosition()).Normalized() * 1000;
	}

	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;

		//Free the bullet and creates an explosion when it goes out of the window
		var windowSize = GetViewport().GetVisibleRect().Size;
		if (Position.X < 0 ||
			Position.X > windowSize.X ||
			Position.Y < 0 ||
			Position.Y > windowSize.Y)
		{
			ExplosionParticles();
			RemoveTrail();
			QueueFree();
		}
	}

	private void ExplosionParticles()
	{
		CpuParticles2D Explosion = ExplosionScene.Instantiate<CpuParticles2D>();
		Explosion.Position = GlobalPosition;
		Explosion.Emitting = true;
		GetParent().AddChild(Explosion);

		Timer explosionTimer = new Timer();
		explosionTimer.WaitTime = 2.0;
		explosionTimer.Timeout += () => Explosion.QueueFree();
		Explosion.AddChild(explosionTimer);
		explosionTimer.Start();
		IntroGameManager.CameraShake(4, 0.2f);
	}

	private void RemoveTrail()
	{
		var trailParticles = GetChildren().OfType<CpuParticles2D>().ToArray();

		foreach (var trail in trailParticles)
		{
			RemoveChild(trail);
			GetParent().AddChild(trail);
		}

		Timer trailTimer = new Timer();
		trailTimer.WaitTime = 2.0;
		trailTimer.Timeout += () =>
		{
			foreach (var trail in trailParticles)
			{
				trail.QueueFree();
				trailParticles = trailParticles.Where(x => x != trail).ToArray();
			}
		};
		GetParent().AddChild(trailTimer);
		trailTimer.Start();
	}
}
