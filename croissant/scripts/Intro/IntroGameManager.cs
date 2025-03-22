using Godot;
using System;
using System.Diagnostics;

public partial class IntroGameManager : Node2D
{
	[Export] private Player Player;
	[Export] private PackedScene BulletScene;
	[Export] private PackedScene EnemyScene;
	[Export] private PackedScene VirusScene;
	[Export] private ColorRect ShaderRect;
	private static Label ScoreLabel;
	[Export] private Label ExportScoreLabel { get => ScoreLabel; set => ScoreLabel = value; }
	private static AnimationPlayer AnimationPlayer;
	[Export] private AnimationPlayer ExportAnimationPlayer { get => AnimationPlayer; set => AnimationPlayer = value; }
	private static Camera2D Camera;
	private Vector2I windowSize = new Vector2I(1920, 1080);
	public static int score = 0;
	public static IntroGameManager Instance;

	[Export] public int MaxScore = 30;
	public override void _Ready()
	{
		GetWindow().Size = GameManager.ScreenSize;
		Instance = this;

		Camera = GetNode<Camera2D>("Camera");
		Player.Position = windowSize / 2;

		SpawnEnemy();
	}

	private Timer enemySpawnTimer;

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Shoot();
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
		{
			UpdateShaders();
		}

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
			float intensityFactor = 1.0f - Mathf.Exp(-5.0f * t);

			SM.SetShaderParameter("shake_power", 0.03f * intensityFactor);
			SM.SetShaderParameter("shake_rate", 0.2f * intensityFactor);
			SM.SetShaderParameter("shake_speed", 5f * intensityFactor);
		}
	}

	private void SpawnEnemy()
	{
		// Initializes the position of the enemies outside of the screen
		Enemy Enemy = EnemyScene.Instantiate<Enemy>();
		float randAngle = Mathf.DegToRad(GD.RandRange(0, 360));
		Enemy.Position = new Vector2(
			Player.Position.X + (float)Math.Cos(randAngle) * windowSize.X,
			Player.Position.Y + (float)Math.Sin(randAngle) * windowSize.Y);
		AddChild(Enemy);
	}

	private void Shoot()
	{
		// Shoots a bullet in direction of the mouse cursor
		Bullet Bullet = BulletScene.Instantiate<Bullet>();
		Node2D BulletPosition = Player.GetNode<Node2D>("BulletPosition");
		Bullet.Position = BulletPosition.GlobalPosition;
		Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
		AddChild(Bullet);
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
