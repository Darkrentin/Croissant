using Godot;
using System;
using System.Security.Cryptography.X509Certificates;

public partial class Enemy : StaticBody2D
{
	[Export] AnimatedSprite2D EnemySprite;
	[Export] CollisionShape2D CollisionShape;
	[Export] float Speed = 110.0f;
	Player Player;
	float RotationSpeed;
	Vector2 velocity;

	int SpriteFrames = 4;

	public override void _Ready()
	{
		// 0: triangle, 1: square, 2: pentagon, 3: hexagon
		EnemySprite.Frame = Lib.rand.Next(0, SpriteFrames);

		Player = GetParent().GetNode<Player>("Player");
		Rotation = (float)Lib.GetRandomNormal(0, 360);
		RotationSpeed = (float)Lib.GetRandomNormal(-2f, 2f);

		Vector2 direction = (Player.GlobalPosition - GlobalPosition).Normalized();
		velocity = direction * Speed;
	}


	public override void _Process(double delta)
	{
		Position += velocity * (float)delta;
		Rotation += RotationSpeed * (float)delta;
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Bullet bullet && bullet.Alive)
		{
			if (EnemySprite.Frame < SpriteFrames - 1)
			{
				EnemySprite.Frame++;
				bullet.EnemyExplosion.Emitting = true;
			}
			else
			{
				IntroGameManager.CameraShake(8, 0.35f);
				bullet.EnemyExplosion.Emitting = true;
				QueueFree();
			}

			bullet.BulletCollide();
		}
		else if (body is Player player)
		{
			IntroGameManager.CameraShake(8, 0.35f);
			velocity = -velocity;
		}
	}
}
