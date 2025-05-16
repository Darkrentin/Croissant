using Godot;

public partial class Player3D : CharacterBody3D
{
	[Export] private RayCast3D RayCast3D;
	[Export] private AnimationTree AnimationTree;
	[Export] private float MoveSpeed = 5.0f;
	[Export] private float RotationSpeed = 2.0f;
	private PackedScene BulletHitScene;
	private AnimationNodeStateMachinePlayback AnimationPlayer;
	private Timer ShootTimer;
	private bool CanShoot = true;
	private string ShootAnimation = "ShootAndReload";
	private float ShootCooldown = 0.9f;
	[Export] private float Gravity = 9.8f;

	public override void _Ready()
	{
		AnimationPlayer = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		ShootTimer = new Timer();
		ShootTimer.Timeout += () => CanShoot = true;
		ShootTimer.WaitTime = ShootCooldown;
		ShootTimer.OneShot = true;
		Input.MouseMode = Input.MouseModeEnum.Captured;
		BulletHitScene = GD.Load<PackedScene>("res://scenes/FinalLevel/Particles/BulletHit.tscn");
		AddChild(ShootTimer);
	}

	public override void _PhysicsProcess(double delta)
	{
		float z_movement = Input.GetActionStrength("Backward") - Input.GetActionStrength("Forward");
		float rotate = Input.GetActionStrength("LeftRot") - Input.GetActionStrength("RightRot");

		Rotation = new Vector3(Rotation.X, Rotation.Y + RotationSpeed * rotate * (float)delta, Rotation.Z);
		Vector3 direction = Transform.Basis.Z;
		Velocity = z_movement * direction * MoveSpeed;

		Vector3 horizontalVelocity = z_movement * direction * MoveSpeed;

		if (!IsOnFloor())
			Velocity = new Vector3(horizontalVelocity.X, Velocity.Y - Gravity * (float)delta * 10, horizontalVelocity.Z);
		else
			Velocity = new Vector3(horizontalVelocity.X, 0, horizontalVelocity.Z);

		MoveAndSlide();

		AnimationPlayer.Travel(Velocity.LengthSquared() > 0 ? "Run" : "RESET");
		if (Input.IsActionPressed("Shoot") && CanShoot)
		{
			CanShoot = false;
			Shoot();
			ShootTimer.WaitTime = ShootCooldown;
			ShootTimer.Start();
		}
	}

	private void Shoot()
	{
		AnimationPlayer.Start(ShootAnimation, reset: true);

		RayCast3D.ForceRaycastUpdate();
		var collider = RayCast3D.GetCollider();

		if (collider is Node3D Body)
		{
			GpuParticles3D bulletHitInstance = BulletHitScene.Instantiate<GpuParticles3D>();
			FinalLevel.Instance.AddChild(bulletHitInstance);
			bulletHitInstance.GlobalPosition = RayCast3D.GetCollisionPoint();
			if (Body is Enemy3D Enemy)
				Enemy.OnBulletCollide();
			else if (Body is Objective Obj)
				Obj.Break();
			else if (Body is Virus3D Virus)
				Virus.TakeDamage();
			else if (Body is FloppyDisk floppyDisk)
				floppyDisk.QueueFree();
		}
	}
}
