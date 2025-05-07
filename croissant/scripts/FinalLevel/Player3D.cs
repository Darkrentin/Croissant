using Godot;

public partial class Player3D : CharacterBody3D
{
	[Export] private AnimationTree AnimationTree;
	[Export] private Node3D Bullet3DSpawnPosition;
	[Export] private PackedScene Bullet3DScene;
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
		if (Input.IsActionPressed("Shoot") && CanShoot)
		{
			CanShoot = false;
			Shoot(delta);
			ShootTimer.Start();
		}
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
	}

	private void Shoot(double delta)
	{
		AnimationPlayer.Travel("Shoot");

		Bullet3D Bullet3D = Bullet3DScene.Instantiate<Bullet3D>();
		Bullet3D.Position = Bullet3DSpawnPosition.GlobalPosition;
		Bullet3D.Rotation = Rotation;
		Bullet3D.Velocity = (Bullet3DSpawnPosition.GlobalPosition - (GlobalPosition + new Vector3(0, Scale.Y, 0))).Normalized() * 30000 * (float)delta;
		GetParent().AddChild(Bullet3D);
	}
}