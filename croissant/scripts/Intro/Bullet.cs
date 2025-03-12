using Godot;
using System;

public partial class Bullet : StaticBody2D
{
	PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Explosion.tscn");
	[Export] Vector2 Velocity = new Vector2(100.0f, 100.0f);


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Velocity = -(GetParent().GetNode<Player>("Player").GlobalPosition - GetGlobalMousePosition()).Normalized() * 800;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
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
			QueueFree();
		}
	}
}
