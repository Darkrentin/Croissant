using Godot;

public partial class Bullet : StaticBody2D
{
	[Export] private Vector2 Velocity;
	[Export] private Polygon2D Polygon2D;
	[Export] private Timer Timer = new Timer();
	[Export] private CpuParticles2D Trail1;
	[Export] private CpuParticles2D Trail2;
	[Export] private CpuParticles2D Trail3;
	[Export] public CpuParticles2D WallExplosion;
	[Export] public CpuParticles2D EnemyHit;
	[Export] public CpuParticles2D EnemyExplosion;
	[Export] public AudioStreamPlayer WallHit;
	public bool Alive = true;

	public override void _Ready()
	{
		//Get the velocity base on the mouse position
		Velocity = -(GetParent().GetNode<Player>("Player").GlobalPosition - GetGlobalMousePosition()).Normalized() * 1000;
	}

	public override void _Process(double delta)
	{
		//Move the bullet
		Position += Velocity * (float)delta;

		//Free the bullet and creates an explosion when it goes out of the window
		Vector2 windowSize = GetViewport().GetVisibleRect().Size;
		if (Alive && (Position.X < 0 || Position.X > windowSize.X || Position.Y < 0 || Position.Y > windowSize.Y))
		{
			BulletCollide();
			WallExplosion.Emitting = true;
			WallHit.Play();
		}
	}

	//On bullet collision, launches a bullet
	public void BulletCollide()
	{
		Alive = false;
		Polygon2D.Visible = false;
		Velocity = Vector2.Zero;
		Trail1.Emitting = false;
		Trail2.Emitting = false;
		Trail3.Emitting = false;
		Timer.WaitTime = 2.0;
		Timer.Start();
	}

	//On timer timeout, activated throught signals
	public void OnTimerTimeout()
	{
		QueueFree();
	}
}
