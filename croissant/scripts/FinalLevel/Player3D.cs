using Godot;

public partial class Player3D : CharacterBody3D
{
	[Export] private RayCast3D RayCast3D;
	[Export] private AnimationTree AnimationTree;
	[Export] private GpuParticles3D BulletHitParticles;
	[Export] private float MoveSpeed = 5.0f;
	[Export] private float RotationSpeed = 2.0f;
	public AnimationNodeStateMachinePlayback AnimationPlayer;
	private Timer ShootTimer;
	private bool CanShoot = true;

	public override void _Ready()
	{
		AnimationPlayer = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		ShootTimer = new Timer();
		ShootTimer.Timeout += () => CanShoot = true;
		ShootTimer.WaitTime = 0.95f;
		ShootTimer.OneShot = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		AddChild(ShootTimer);
	}

	public override void _Process(double delta)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		float z_movement = Input.GetActionStrength("Backward") - Input.GetActionStrength("Forward");
		float rotate = Input.GetActionStrength("LeftRot") - Input.GetActionStrength("RightRot");

		Rotation = new Vector3(Rotation.X, Rotation.Y + RotationSpeed * rotate * (float)delta, Rotation.Z);
		Vector3 direction = Transform.Basis.Z;
		Velocity = z_movement * direction * MoveSpeed;

		MoveAndSlide();

		AnimationPlayer.Travel(Velocity.LengthSquared() > 0 ? "Run" : "RESET");
		if (Input.IsActionPressed("Shoot") && CanShoot)
		{
			CanShoot = false;
			Shoot();
			ShootTimer.Start();
		}
	}

	private void Shoot()
	{
		AnimationPlayer.Travel("Shoot");

		RayCast3D.ForceRaycastUpdate();
		if (RayCast3D.GetCollider() is Node3D Body)
		{
			BulletHitParticles.GlobalPosition = RayCast3D.GetCollisionPoint();
			BulletHitParticles.Emitting = true;
			if (Body is Enemy3D Enemy)
				Enemy.OnBulletCollide();
		}
	}
}