using Godot;
using System;

public partial class Player3D : CharacterBody3D
{
	[Export] private AnimationTree AnimationTree;
	[Export] private Node3D Bullet3DSpawnPosition;
	[Export] private PackedScene Bullet3DScene;
	[Export] private PackedScene Enemy3DScene;
	[Export] private float MoveSpeed = 6.0f;
	[Export] private float RotationSpeed = 4.0f;
	public AnimationNodeStateMachinePlayback AnimationPlayer;
	private Timer ShootTimer = new Timer();
	private bool CanShoot = true;

	public override void _Ready()
	{
		AnimationPlayer = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		ShootTimer.Timeout += () => CanShoot = true;
		ShootTimer.WaitTime = 0.95f;
		ShootTimer.OneShot = true;
		AddChild(ShootTimer);
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Shoot"))
		{
			if (CanShoot)
			{
				CanShoot = false;
				Shoot(delta);
				ShootTimer.Start();
			}
		}
	}

	public override void _PhysicsProcess(double delta)
	{
		float z_movement = Input.GetActionStrength("Backward") - Input.GetActionStrength("Forward");
		float rotate = Input.GetActionStrength("LeftRot") - Input.GetActionStrength("RightRot");

		Rotation = new Vector3(Rotation.X, Rotation.Y + RotationSpeed * rotate * (float)delta, Rotation.Z);
		Vector3 direction = new Vector3(0, 0, 1).Rotated(new Vector3(0, 1, 0), Rotation.Y);
		Vector3 motion = z_movement * direction * MoveSpeed * (float)delta;

		MoveAndCollide(motion);

		if (motion.LengthSquared() > 0)
			AnimationPlayer.Travel("Run");
		else
			AnimationPlayer.Travel("RESET");
	}

	private void Shoot(double delta)
	{
		AnimationPlayer.Travel("Shoot");

		Bullet3D Bullet3D = Bullet3DScene.Instantiate<Bullet3D>();
		Bullet3D.Position = Bullet3DSpawnPosition.GlobalPosition;
		Bullet3D.Rotation = Rotation;
		Bullet3D.Velocity = (Bullet3DSpawnPosition.GlobalPosition - (GlobalPosition + new Vector3(0, Scale.Y, 0))).Normalized() * 3000 * (float)delta;
		GetParent().AddChild(Bullet3D);
	}
}