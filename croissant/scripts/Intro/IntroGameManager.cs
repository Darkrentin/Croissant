using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	[Export] private Player Player;
	[Export] private PackedScene BulletScene;
	[Export] private PackedScene EnemyScene;
	[Export] private PackedScene VirusScene;
	[Export] private ColorRect ShaderRect;
	private static Label ScoreLabel;
	[Export] public Label ExportScoreLabel {get => ScoreLabel; set => ScoreLabel = value;}	
	private static AnimationPlayer AnimationPlayer;
	[Export] public AnimationPlayer ExportAnimationPlayer {get => AnimationPlayer; set => AnimationPlayer = value;}
	private static Camera2D Camera;
	private Vector2I windowSize = new Vector2I(1920, 1080);
	public static int score = 0;

	public override void _Ready()
	{
		GetWindow().Size = GameManager.ScreenSize;

		Camera = GetNode<Camera2D>("Camera");
		Player.Position = windowSize / 2;

	}

	private Timer enemySpawnTimer;

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Shoot();
		}
		// Creates an enemy every 0.8 to 1.2 seconds
		if (enemySpawnTimer == null)
		{
			enemySpawnTimer = new Timer();
			enemySpawnTimer.WaitTime = Lib.GetRandomNormal(0.8f, 1.2f);
			enemySpawnTimer.OneShot = false;
			enemySpawnTimer.Timeout += SpawnEnemy;
			AddChild(enemySpawnTimer);
			enemySpawnTimer.Start();
		}
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
			SM.SetShaderParameter("shake_power", score * 0.002f);
			SM.SetShaderParameter("shake_rate", score * 0.012f);
			SM.SetShaderParameter("shake_speed", score * 0.03f);
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
