using Godot;
using System;

public partial class Enemy : StaticBody2D
{
	[Export] Polygon2D Polygon;
	[Export] CollisionPolygon2D CollisionPolygon;
	[Export] float Speed = 110.0f;
	Player Player;
	float RotationSpeed;
	int shape;

	private readonly Vector2[] triangleShape =
	{
		new Vector2(0, -60),
		new Vector2(52, 30),
		new Vector2(-52, 30)
	};

	private readonly Vector2[] squareShape =
	{
		new Vector2(-45, -45),
		new Vector2(45, -45),
		new Vector2(45, 45),
		new Vector2(-45, 45)
	};

	private readonly Vector2[] pentagonShape =
	{
		new Vector2(0, -50),
		new Vector2(48, -15),
		new Vector2(30, 40),
		new Vector2(-30, 40),
		new Vector2(-48, -15)
	};

	private readonly Vector2[] hexagonShape =
	{
		new Vector2(0, -48),
		new Vector2(42, -24),
		new Vector2(42, 24),
		new Vector2(0, 48),
		new Vector2(-42, 24),
		new Vector2(-42, -24)
	};

	public override void _Ready()
	{
		shape = Lib.rand.Next(0, 4);
		SetShape(shape);

		Player = GetParent().GetNode<Player>("Player");
		Rotation = (float)Lib.GetRandomNormal(0, 360);
		RotationSpeed = (float)Lib.GetRandomNormal(-2f, 2f);

		CollisionPolygon.Position = Vector2.Zero;
		Polygon.Position = Vector2.Zero;
		CollisionPolygon.Rotation = 0;
		Polygon.Rotation = 0;
	}

	private void SetShape(int shapeType)
	{
		Vector2[] points = shapeType switch
		{
			0 => triangleShape,
			1 => squareShape,
			2 => pentagonShape,
			3 => hexagonShape,
			_ => triangleShape
		};

		Polygon.Polygon = points;

		CallDeferred(nameof(UpdateCollisionPolygon), points);
	}

	private void UpdateCollisionPolygon(Vector2[] points)
	{
		CollisionPolygon.Polygon = points;
	}

	public override void _Process(double delta)
	{
		Vector2 direction = (Player.GlobalPosition - GlobalPosition).Normalized();
		Vector2 velocity = direction * Speed;
		Position += velocity * (float)delta;
		Rotation += RotationSpeed * (float)delta;
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Bullet bullet && bullet.Alive)
		{
			if (shape > 0)
			{
				shape--;
				SetShape(shape);

				//Sreen shake when shape decreases
				//float shakeIntensity = 3 + (3 - shape);
				//float shakeDuration = 0.2f + (0.05f * (3 - shape));
				//IntroGameManager.CameraShake(shakeIntensity, shakeDuration);

				bullet.EnemyExplosion.Emitting = true;
			}
			else if (shape == 0)
			{
				IntroGameManager.CameraShake(8, 0.35f);
				bullet.EnemyExplosion.Emitting = true;
				QueueFree();
			}

			bullet.BulletCollide();
		}
	}
}
