using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] float Speed = 120.0f;
	Player Player;
	float RotationSpeed;
	public override void _Ready()
	{
		Player = GetParent().GetNode<Player>("Player");
		Rotation = (float)Lib.GetRandomNormal(0, 360);
		RotationSpeed = (float)Lib.GetRandomNormal(-2f, 2f);
	}

	public override void _Process(double delta)
	{
		//Move the enemy towards the player
		Vector2 direction = (Player.GlobalPosition - GlobalPosition).Normalized();
		Vector2 velocity = direction * Speed;
		Position += velocity * (float)delta;
		Rotation += RotationSpeed * (float)delta;
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Bullet bullet && bullet.Alive)
		{
			IntroGameManager.CameraShake(6, 0.3f);
			bullet.BulletCollide();
			bullet.EnemyExplosion.Emitting = true;
			QueueFree();
		}
	}
}
