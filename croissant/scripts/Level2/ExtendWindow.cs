using Godot;
using System;

public partial class ExtendWindow : AttackWindow
{

	public Vector2I TargetPosition;
	public Vector2I TargetSize;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void Move()
	{
		const float MoveTime = 0.5f;
		const float MarginTime = 0.1f;
		Vector2I margin = Parent.CursorWindow.Size/2;
		Vector2I CursorPosition = Parent.CursorWindow.Position;
		TargetPosition = new Vector2I(Lib.rand.Next(CursorPosition.X - margin.X, CursorPosition.X + margin.X), Lib.rand.Next(CursorPosition.Y - margin.Y, CursorPosition.Y + margin.Y)) - Parent.CursorWindow.Size/2;
		windowPosition = TargetPosition;
		windowSize = Size;
		StartTransition(TargetPosition,MoveTime-MarginTime);

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);

		TargetSize = Size + Lib.GetScreenSize(0.2f, 0.2f);
		ShowVisualCollision(TargetSize, StartResize(TargetSize,0));
		IsResizing = false;

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;
		StartResize(TargetSize, ResizeTime);
		HideVisualCollision();

		Timer.WaitTime = ResizeTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		const float ResetTime = 1f;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, ResetTime);
		StartTransition(windowPosition, ResetTime, reset: true);
		
		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
	}
}
