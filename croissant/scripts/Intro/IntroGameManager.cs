using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	[Export] private Player Player;
	[Export] private PackedScene BulletScene;
	[Export] private PackedScene EnemyScene;
	[Export] private PackedScene VirusScene;
	[Export] private ColorRect ExportShaderRect { get => ShaderRect; set => ShaderRect = value; }
	[Export] private PackedScene ExportGameExplosion { get => GameExplosionScene; set => GameExplosionScene = value; }
	[Export] private Label ExportScoreLabel { get => ScoreLabel; set => ScoreLabel = value; }
	[Export] private AnimationPlayer ExportAnimationPlayer { get => AnimationPlayer; set => AnimationPlayer = value; }
	[Export] public int MaxScore = 30;
	[Export] private TextureRect IntroRect;
	[Export] public AudioStreamPlayer PewPewSound;
	public static AudioStreamPlayer StaticGameExplosionSound;
	[Export] public AudioStreamPlayer GameExplosionSound { get => StaticGameExplosionSound; set => StaticGameExplosionSound = value; }
	[Export] public AudioStreamPlayer GlitchSound;
	private static ColorRect ShaderRect;
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
	public static IntroGameManager Instance;
	[Export] public Node2D GameNode;

	public override void _Ready()
	{
		GetWindow().Size = GameManager.ScreenSize + new Vector2I(1, 1);
		IntroRect.Position = new Vector2(0, GetViewportRect().Size.Y - IntroRect.Size.Y);
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
			enemySpawnTimer.WaitTime = Lib.GetRandomNormal(0.8f, 1f);
			enemySpawnTimer.OneShot = false;
			enemySpawnTimer.Timeout += SpawnEnemy;
			AddChild(enemySpawnTimer);
			enemySpawnTimer.Start();
		}

		if (score >= 10)
			UpdateShaders();
	}

	public static void AddScore()
	{
		score++;
		ScoreLabel.Text = score.ToString();
		AnimationPlayer.Play("ScoreUp");
	}

	private void UpdateShaders()
	{
		// Glitches the screen based on the score
		if (ShaderRect != null && ShaderRect.Material is ShaderMaterial SM)
		{
			float t = Mathf.Min((score - 10) / 20.0f, 1.0f);
			float intensityFactor = 1f - Mathf.Exp(-5.0f * t);

			SM.SetShaderParameter("shake_power", 0.025f * intensityFactor);
			SM.SetShaderParameter("shake_rate", 0.15f * intensityFactor);
			SM.SetShaderParameter("shake_speed", 4f * intensityFactor);
		}
	}

	public static void EndActions()
	{
		// Glitches the screen based on the score
		if (ShaderRect != null && ShaderRect.Material is ShaderMaterial SM)
		{
			SM.SetShaderParameter("shake_power", 0.1f);
			SM.SetShaderParameter("shake_rate", 0.2f);
			SM.SetShaderParameter("shake_speed", 5f);
		}
		CameraShake(20.0f, 0.5f);
		CpuParticles2D GameExplosion = GameExplosionScene.Instantiate<CpuParticles2D>();
		GameExplosion.Position = new Vector2(1920 / 2, 1080 / 2);
		Instance.AddChild(GameExplosion);
		GameExplosion.Emitting = true;
		Instance.ExplosionTimer.Start();
		StaticGameExplosionSound.Play();
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
	}

	private void Shoot()
	{
		PewPewSound.Play();
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
}
