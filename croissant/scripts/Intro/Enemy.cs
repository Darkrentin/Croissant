using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] private AnimatedSprite2D EnemySprite;
	[Export] private CollisionShape2D CollisionShape;
	[Export] private float Speed = 110.0f;
	private Player Player;
	private float RotationSpeed;
	private Vector2 velocity;
	private int SpriteFrames = 4;

	public override void _Ready()
	{
		// 3 : Hexagon
		// 2 : Pentagon
		// 1 : Square
		// 0 : Triangle
		EnemySprite.Frame = Lib.rand.Next(0, SpriteFrames);

		Player = GetParent().GetNode<Player>("Player");
		Rotation = Lib.GetRandomNormal(0f, 360f);
		RotationSpeed = Lib.GetRandomNormal(-2f, 2f);

		// Make the shape go toward the player
		Vector2 direction = (Player.GlobalPosition - GlobalPosition).Normalized();
		velocity = direction * Speed;
	}


	public override void _Process(double delta)
	{
		// Move the enemy
		Position += velocity * (float)delta;
		Rotation += RotationSpeed * (float)delta;
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Bullet bullet && bullet.Alive)
		{
			if (EnemySprite.Frame < SpriteFrames - 1)
			{
				// Decreases the shape on collision with the bullet
				EnemySprite.Frame++;
				bullet.EnemyExplosion.Emitting = true;
			}
			else
			{
				IntroGameManager.score++;

				// Explodes when it is in the triangle shape
				IntroGameManager.CameraShake(8, 0.35f);
				bullet.EnemyExplosion.Emitting = true;
				QueueFree();
			}

			bullet.BulletCollide();
		}
		else if (body is Player player)
		{
			// Bounces back on collision with the player
			IntroGameManager.CameraShake(8, 0.35f);
			velocity = -velocity;
		}
	}
}
