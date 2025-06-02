using Godot;
using System;

public partial class SpikeWindow : AttackWindow
{
	[Export] public AudioStreamPlayer AttackSound;
	public Vector2I TargetPosition;

	public override void _Ready()
	{
		base._Ready();
		VisualCollision.Color = Colors.Blue;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void Start()
	{
		base.Start();
	}
	public override void Move()
	{
		const float MoveTime = 0.2f;
		//const float margin = 0.1f;
		TargetPosition = ClampToScreen(CursorPosition - Level2.CursorWindow.Size / 2);
		StartExponentialTransition(TargetPosition, MoveTime, reset: true);
		//windowPosition = TargetPosition;

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1.5f;
		StartShake(ShakeTime, 5); //FIND WHY THE WINDOWS DISEAPPEAR WHEN I DON'T USE THE SHAKE !

		ShowVisualCollision(Size, TargetPosition, ShakeTime);

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		AttackSound.Play();
		const float ResizeTime = 0.1f;
		const float AttackDuration = 4.5f;

		Timer.WaitTime = ResizeTime + AttackDuration + Lib.GetRandomNormal(0.5f, 3.0f); ;
		base.Attack();
	}

	public override void Reload()
	{
		AttackSound.Stop();
		HideVisualCollision();
		Timer.WaitTime = 0.1f; // time to wait before restarting
		base.Reload();
	}
}
