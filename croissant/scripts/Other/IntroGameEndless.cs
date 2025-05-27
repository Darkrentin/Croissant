using Godot;
using System;

public partial class IntroGameEndless : Node2D
{
	[Export] private Player Player;
	[Export] private PackedScene BulletScene;
	[Export] private PackedScene EnemyScene;
	[Export] private PackedScene ExportGameExplosion { get => GameExplosionScene; set => GameExplosionScene = value; }
	[Export] private Label ExportScoreLabel { get => ScoreLabel; set => ScoreLabel = value; }
	[Export] private AnimationPlayer ExportAnimationPlayer { get => AnimationPlayer; set => AnimationPlayer = value; }
	private static PackedScene GameExplosionScene;
	private static Label ScoreLabel;
	private static AnimationPlayer AnimationPlayer;
	private static Camera2D Camera;
	private Vector2I windowSize = new Vector2I(1920, 1080);
	private Timer ShootTimer = new Timer();
	private bool CanShoot = true;
	private Timer enemySpawnTimer;
	private Timer ExplosionTimer;
	public static int score = 0;
	public static IntroGameEndless Instance;
	[Export] public Node2D GameNode;
	
	// Difficulty progression variables
	private float baseEnemySpawnTime = 1.0f;  // Initial spawn time
	private float minEnemySpawnTime = 0.2f;   // Minimum spawn time
	private float baseShootCooldown = 0.15f;  // Initial shoot cooldown
	private float minShootCooldown = 0.05f;   // Minimum shoot cooldown
	private int difficultyIncreaseInterval = 20; // Increase difficulty every 5 points

	public override void _Ready()
	{
		GetWindow().Size = GameManager.ScreenSize + new Vector2I(1, 1);
		Instance = this;

		Camera = GetNode<Camera2D>("Camera");

		Player.Position = GameManager.ScreenSize / 2;

		ShootTimer.Timeout += () => CanShoot = true;
		ShootTimer.WaitTime = 0.15f;
		ShootTimer.OneShot = true;
		AddChild(ShootTimer);

		ExplosionTimer = new Timer();
		ExplosionTimer.WaitTime = 3f;
		ExplosionTimer.Timeout += () =>
		{
			Instance.GetParent().QueueFree();
		};
		ExplosionTimer.OneShot = true;
		AddChild(ExplosionTimer);

		SpawnEnemy();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Shoot"))
		{
			if (CanShoot)
			{
				CanShoot = false;
				Shoot();
				ShootTimer.Start();
			}
		}
		// Creates an enemy every 0.8 to 1 seconds
		if (enemySpawnTimer == null)
		{
			enemySpawnTimer = new Timer();
			enemySpawnTimer.WaitTime = GetCurrentEnemySpawnTime();
			enemySpawnTimer.OneShot = false;
			enemySpawnTimer.Timeout += SpawnEnemy;
			AddChild(enemySpawnTimer);
			enemySpawnTimer.Start();
		}

	}

	public static void AddScore()
	{
		score++;
		ScoreLabel.Text = score.ToString();
		AnimationPlayer.Play("ScoreUp");
		
		// Update difficulty when score increases
		Instance.UpdateDifficulty();
	}

	private void SpawnEnemy()
	{
		// Initializes the position of the enemies outside of the screen
		Enemy Enemy = EnemyScene.Instantiate<Enemy>();
		float randAngle = Mathf.DegToRad(GD.RandRange(0, 360));
		Enemy.Position = new Vector2(
			Player.Position.X + (float)Math.Cos(randAngle) * windowSize.X,
			Player.Position.Y + (float)Math.Sin(randAngle) * windowSize.Y);
		GameNode.AddChild(Enemy);
		Enemy.Endless = true;

		// Update spawn timer for next enemy
		if (enemySpawnTimer != null)
			enemySpawnTimer.WaitTime = GetCurrentEnemySpawnTime();
			
	}

	private void Shoot()
	{
		// Shoots a bullet in direction of the mouse cursor
		Bullet Bullet = BulletScene.Instantiate<Bullet>();
		Node2D BulletPosition = Player.GetNode<Node2D>("BulletPosition");
		Bullet.Position = BulletPosition.GlobalPosition;
		Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
		GameNode.AddChild(Bullet);
	}

	public static void CameraShake(float intensity, float duration)
	{
		float screenFactor = GameManager.ScreenSize.Y / 3072f;
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
	
	// Difficulty progression methods
	private float GetCurrentEnemySpawnTime()
	{
		// Reduce spawn time based on score, but not below minimum
		float difficultyMultiplier = 1.0f - (score / (float)difficultyIncreaseInterval) * 0.1f;
		float currentSpawnTime = baseEnemySpawnTime * difficultyMultiplier;
		return Mathf.Max(currentSpawnTime, minEnemySpawnTime);
	}
	
	private float GetCurrentShootCooldown()
	{
		// Reduce shoot cooldown based on score, but not below minimum
		float difficultyMultiplier = 1.0f - (score / (float)difficultyIncreaseInterval) * 0.05f;
		float currentCooldown = baseShootCooldown * difficultyMultiplier;
		return Mathf.Max(currentCooldown, minShootCooldown);
	}
	
	private void UpdateDifficulty()
	{
		// Update shoot timer cooldown
		ShootTimer.WaitTime = GetCurrentShootCooldown();
		
		// Enemy spawn timer is updated in SpawnEnemy method
	}
}
