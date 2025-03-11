using Godot;
using System;

public partial class IntroBullet : StaticBody2D
{
	[Export] Vector2 Velocity = new Vector2(100.0f, 100.0f);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Velocity = -(GetParent().GetNode<IntroPlayer>("IntroPlayer").GlobalPosition - GetGlobalMousePosition()).Normalized() * 400;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;
		Velocity -= Velocity * (float)delta * 0.2f;
		if (Velocity.Length() < 100)
		{
			QueueFree();
		}
	}
}
