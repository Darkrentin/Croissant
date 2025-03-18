using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	[Export] Player Player;
	[Export] PackedScene BulletScene;
	[Export] PackedScene EnemyScene;
	[Export] PackedScene VirusScene;
	private static Camera2D Camera;
	private static Vector2I screenSize;

	Vector2I windowSize;
	private Random random = new Random();

	public override void _Ready()
	{
		screenSize = DisplayServer.ScreenGetSize();
		GetWindow().Size = screenSize;
		Vector2I windowSize = new Vector2I(1920, 1080);

		Camera = GetNode<Camera2D>("Camera");
		Player.Position = windowSize / 2;


		Virus Virus = VirusScene.Instantiate<Virus>();
		AddChild(Virus);
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
			enemySpawnTimer.WaitTime = (float)Lib.GetRandomNormal(0.8f, 1.5f);
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
		float randAngleRad = (float)(random.Next(0, 360) * Math.PI / 180.0);
		float spawnDistance = Math.Max(screenSize.X, screenSize.Y) * 0.5f;
		Enemy.Position = new Vector2(
			Player.Position.X + (float)Math.Cos(randAngleRad) * spawnDistance,
			Player.Position.Y + (float)Math.Sin(randAngleRad) * spawnDistance);

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
