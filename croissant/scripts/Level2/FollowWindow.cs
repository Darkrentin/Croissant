using Godot;
using System;

public partial class FollowWindow : AttackWindow
{
	public Vector2I TargetPosition;

	public override void _Ready()
	{

		base._Ready();
		VisualCollision.Color = Colors.Green;
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
		const float MoveTime = 0.1f;
		//const float margin = 0.1f;
		
		//windowPosition = TargetPosition;

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5); //FIND WHY THE WINDOWS DISEAPPEAR WHEN I DON'T USE THE SHAKE !
		
        TargetPosition = CursorPosition - Level2.CursorWindow.Size / 2;
		ShowVisualCollision(Size, TargetPosition, ShakeTime);

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.1f;
		
        StartExponentialTransition(TargetPosition, ResizeTime, reset: true);

		Timer.WaitTime = ResizeTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		HideVisualCollision();
		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
	}
}