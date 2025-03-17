using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	private static Camera2D Camera;
	private static Vector2I screenSize;
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("uid://1jqgd3hgljd8");
	PackedScene EnemyScene = ResourceLoader.Load<PackedScene>("uid://37mav66th0um");
	Player Player;
	Vector2I windowSize;
	private Random random = new Random();

	public override void _Ready()
	{
		screenSize = DisplayServer.ScreenGetSize();
		GetWindow().Size = screenSize;
		Vector2I windowSize = new Vector2I(1920, 1080);

		Camera = GetNode<Camera2D>("Camera");
		Player = GetNode<Player>("Player");
		Player.Position = windowSize / 2;
	}

	private Timer enemySpawnTimer;

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Shoot();
		}

		//Creates an enemy every 0.5 to 1 seconds
		if (enemySpawnTimer == null)
		{
			enemySpawnTimer = new Timer();
			enemySpawnTimer.WaitTime = (float)Lib.GetRandomNormal(1f, 2.0f);
			enemySpawnTimer.OneShot = false;
			enemySpawnTimer.Timeout += SpawnEnemy;
			AddChild(enemySpawnTimer);
			enemySpawnTimer.Start();
		}
	}

	private void SpawnEnemy()
	{
		Enemy Enemy = EnemyScene.Instantiate<Enemy>();
		//Initialize the position outside of the screen
		int Border = random.Next(0, 4);
		if (Border == 0)
			Enemy.Position = new Vector2((float)Lib.GetRandomNormal(-100, screenSize.X + 100), -100);
		else if (Border == 1)
			Enemy.Position = new Vector2((float)Lib.GetRandomNormal(-100, screenSize.X + 100), screenSize.Y + 100);
		else if (Border == 2)
			Enemy.Position = new Vector2(-100, (float)Lib.GetRandomNormal(-100, screenSize.Y + 100));
		else
			Enemy.Position = new Vector2(screenSize.X + 100, (float)Lib.GetRandomNormal(-100, screenSize.Y + 100));
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
		float screenFactor = screenSize.Y / 3072f;
		float scaledIntensity = intensity * screenFactor;

		var tween = Camera.CreateTween();
		Vector2 startPosition = Camera.Offset;

		for (int i = 0; i < 10; i++)
		{
			tween.TweenProperty(Camera, "offset", startPosition + new Vector2(
				(float)Lib.GetRandomNormal(-scaledIntensity, scaledIntensity),
				(float)Lib.GetRandomNormal(-scaledIntensity, scaledIntensity)),
				duration / 20);

			tween.TweenProperty(Camera, "offset", startPosition + new Vector2(
				(float)Lib.GetRandomNormal(-scaledIntensity / 2, scaledIntensity / 2),
				(float)Lib.GetRandomNormal(-scaledIntensity / 2, scaledIntensity / 2)),
				duration / 20);
		}

		tween.TweenProperty(Camera, "offset", startPosition, duration / 10);
	}
}
