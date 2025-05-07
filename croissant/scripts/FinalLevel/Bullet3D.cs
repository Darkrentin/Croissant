using Godot;

public partial class Bullet3D : CharacterBody3D
{
	[Export] private MeshInstance3D Mesh;
	[Export] private OmniLight3D OmniLight3D;
	private Timer Timer = new Timer();

	public bool Alive = true;

	public override void _Ready()
	{
		Timer.Timeout += OnTimerTimeout;
		AddChild(Timer);
	}

	public override void _Process(double delta)
	{
		Position += Velocity * (float)delta;
	}

	public void OnBodyCollision(Node3D Body)
	{
		if (Body is Player3D) return;
		if (Body is Enemy3D Enemy && Enemy.Alive)
		{
			Enemy.OnBulletCollide();
			//EnemyHit.Emitting = true;
		}
		else
		{
			//WallExplosion.Emitting = true;
		}
		OmniLight3D.Visible = false;
		Alive = false;
		Mesh.Visible = false;
		Velocity = Vector3.Zero;
		Timer.WaitTime = 2.0;
		Timer.Start();
	}

	public void OnTimerTimeout()
	{
		QueueFree();
	}
}