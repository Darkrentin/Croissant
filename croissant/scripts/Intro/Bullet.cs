using Godot;
using System;
using System.Linq;

public partial class Bullet : StaticBody2D
{
	PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Explosion.tscn");
	[Export] Vector2 Velocity = new Vector2(100.0f, 100.0f);

	public override void _Ready()
	{
		Velocity = -(GetParent().GetNode<Player>("Player").GlobalPosition - GetGlobalMousePosition()).Normalized() * 800;
	}

	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;
		//Velocity -= Velocity * (float)delta * 0.2f;

		//Free the bullet and creates an explosion when it goes out of the window
		var windowSize = GetViewport().GetVisibleRect().Size;
		if (Position.X < 0 ||
			Position.X > windowSize.X ||
			Position.Y < 0 ||
			Position.Y > windowSize.Y)
		{
			CpuParticles2D Explosion = ExplosionScene.Instantiate<CpuParticles2D>();
			Explosion.Position = GlobalPosition;
			Explosion.Emitting = true;
			GetParent().AddChild(Explosion);

			var trailParticles = GetChildren().OfType<CpuParticles2D>().ToArray();

			foreach (var trail in trailParticles)
			{
				RemoveChild(trail);
				GetParent().AddChild(trail);
			}

			Timer explosionTimer = new Timer();
			explosionTimer.WaitTime = 2.0;
			explosionTimer.Timeout += () =>
			{
				Explosion.QueueFree();
				foreach (var trail in trailParticles)
					trail.QueueFree();
			};
			Explosion.AddChild(explosionTimer);
			explosionTimer.Start();

			QueueFree();
		}
	}
}
