using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	private static Camera2D Camera;
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Bullet.tscn");
	PackedScene EnemyScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Enemy.tscn");
	Player Player;
	Vector2I screenSize = DisplayServer.ScreenGetSize();
	Vector2I windowSize;
	public override void _Ready()
	{

		GetWindow().Size = screenSize;
		Vector2I windowSize = GetWindow().Size;

		Camera = GetNode<Camera2D>("Camera");
		Player = GetNode<Player>("Player");
		Player.Position = windowSize / 2;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Shoot();
			CameraShake(3, 0.1f);
		}

		if (Lib.rand.Next(0, 120) == 8)
		{
			SpawnEnemy();
		}
	}

	private void SpawnEnemy()
	{
		Enemy Enemy = EnemyScene.Instantiate<Enemy>();
		Vector2I spawnPosition = new Vector2I(
			(int)Lib.GetRandomNormal(0, screenSize.X),
			(int)Lib.GetRandomNormal(0, screenSize.Y));
		Enemy.Position = spawnPosition;
		AddChild(Enemy);
	}

	private void Shoot()
	{
		Bullet Bullet = BulletScene.Instantiate<Bullet>();
		Node2D BulletPosition = Player.GetNode<Node2D>("BulletPosition");
		Bullet.Position = BulletPosition.GlobalPosition;
		Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
		AddChild(Bullet);
	}

	public static void CameraShake(float intensity, float duration)
	{
		var tween = Camera.CreateTween();
		Vector2 startPosition = Camera.Offset;

		for (int i = 0; i < 10; i++)
		{
			// Apply random offset
			tween.TweenProperty(Camera, "offset", startPosition + new Vector2(
				(float)Lib.GetRandomNormal(-intensity, intensity),
				(float)Lib.GetRandomNormal(-intensity, intensity)),
				duration / 20);

			// Move back half of the distance with the center
			tween.TweenProperty(Camera, "offset", startPosition + new Vector2(
				(float)Lib.GetRandomNormal(-intensity / 2, intensity / 2),
				(float)Lib.GetRandomNormal(-intensity / 2, intensity / 2)),
				duration / 20);
		}

		tween.TweenProperty(Camera, "offset", startPosition, duration / 10);
	}
}
