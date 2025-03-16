using Godot;
using System;

public partial class Bullet : StaticBody2D
{
	[Export] Vector2 Velocity;
	[Export] Polygon2D Polygon2D;
	[Export] CollisionShape2D CollisionShape;
	[Export] Timer Timer = new Timer();
	[Export] CpuParticles2D Trail1;
	[Export] CpuParticles2D Trail2;
	[Export] CpuParticles2D Trail3;
	[Export] CpuParticles2D WallExplosion;
	[Export] public CpuParticles2D EnemyExplosion;
	public bool Alive = true;

	public override void _Ready()
	{
		Velocity = -(GetParent().GetNode<Player>("Player").GlobalPosition - GetGlobalMousePosition()).Normalized() * 1000;
	}

	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;

		//Free the bullet and creates an explosion when it goes out of the window
		Vector2 windowSize = GetViewport().GetVisibleRect().Size;
		if (Position.X < 0 ||
			Position.X > windowSize.X ||
			Position.Y < 0 ||
			Position.Y > windowSize.Y)
		{
			BulletCollide();
			WallExplosion.Emitting = true;
		}
	}

	public void BulletCollide()
	{
		Alive = false;
		Polygon2D.Visible = false;
		Velocity = Vector2.Zero;
		Trail1.Emitting = false;
		Trail2.Emitting = false;
		Trail3.Emitting = false;
		Timer.WaitTime = 2.0;
		Timer.Start();
	}
	public void OnTimerTimeout()
	{
		QueueFree();
	}
}
