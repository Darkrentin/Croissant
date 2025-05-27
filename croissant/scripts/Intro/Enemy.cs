using Godot;

public partial class Enemy : StaticBody2D
{
	[Export] private AnimationPlayer AnimationPlayer;
	[Export] private AnimatedSprite2D EnemySprite;
	[Export] private CollisionShape2D Collision;
	[Export] private float Speed = 110.0f;
	private Player Player;
	private float RotationSpeed;
	private Vector2 velocity;
	private int SpriteFrames = 4;
	private bool Alive = true;
	public bool Endless = false;

	public override void _Ready()
	{
		// 3 : Hexagon
		// 2 : Pentagon
		// 1 : Square
		// 0 : Triangle
		EnemySprite.Frame = Lib.rand.Next(0, SpriteFrames);

		if (EnemySprite.Frame == 3)
			Collision.Position += new Vector2(0, 60);

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

	public void OnAnimationFinished(string anim_name)
	{
		QueueFree();
	}

	public void OnBodyEntered(Node body)
	{
		if (body is Bullet bullet && bullet.Alive && Alive)
		{
			if (EnemySprite.Frame < SpriteFrames - 1)
			{
				// Decreases the shape on collision with the bullet
				EnemySprite.Frame++;
				bullet.EnemyHit.Emitting = true;
			}
			else
			{
				if (Endless)
				{
					IntroGameEndless.AddScore();
					IntroGameEndless.CameraShake(8, 0.35f);
				}
				else{
					// Adds score when the enemy is destroyed
					IntroGameManager.AddScore();
					IntroGameManager.CameraShake(8, 0.35f);
				}
				bullet.EnemyExplosion.Emitting = true;
				Collision.Visible = false;
				velocity = Vector2.Zero;
				Alive = false;
				AnimationPlayer.Play("Depop");
			}

			if (EnemySprite.Frame == 3)
				Collision.Position += new Vector2(0, 60);

			bullet.BulletCollide();
		}
		else if (body is Player player)
		{
			// Bounces back on collision with the player
			if(Endless)
				IntroGameEndless.CameraShake(8, 0.35f);
			else
				IntroGameManager.CameraShake(8, 0.35f);
			velocity = -velocity * 2f;
			int rand = Lib.rand.Next(0, 3);

			if (rand == 0)
				EnemySprite.Modulate = new Color(1, 0, 0);
			else if (rand == 1)
				EnemySprite.Modulate = new Color(0, 1, 0);
			else
				EnemySprite.Modulate = new Color(0, 0, 1);

		}
	}
}
