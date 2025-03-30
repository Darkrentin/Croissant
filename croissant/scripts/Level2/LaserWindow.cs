using Godot;
using System;

public partial class LaserWindow : AttackWindow
{
	protected int side;

	public Vector2I TargetPosition;
	public Vector2I CursorPosition;
	private int nsize = 0;

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

	private Vector2I GetTargetPosition(int side)
	{
		const int margin = 150;
		Lib.Print($"Side: {side}");
		switch (side)
		{
			case (3):
				return new Vector2I(Lib.rand.Next(0, Parent.CursorWindow.CenterPosition.X - margin), Parent.CursorWindow.CenterPosition.Y);
			case (0):
				return new Vector2I(Parent.CursorWindow.CenterPosition.X, Lib.rand.Next(Parent.CursorWindow.CenterPosition.Y + margin, GameManager.ScreenSize.Y));
			case (1):
				return new Vector2I(Lib.rand.Next(Parent.CursorWindow.CenterPosition.X + margin, GameManager.ScreenSize.X), Parent.CursorWindow.CenterPosition.Y);
			case (2):
				return new Vector2I(Parent.CursorWindow.CenterPosition.X, Lib.rand.Next(0, Parent.CursorWindow.CenterPosition.Y - margin));
			default:
				GD.PushError("Invalid side");
				return new Vector2I(0, 0);

		}
	}

	private (Vector2I newSize, Vector2I newPosition) CallResize(int nsize, float time)
	{
		switch (side)
		{
			case (0):
				return StartResizeUp(nsize, time);
			case (1):
				return StartResizeLeft(nsize, time);
			case (2):
				return StartResizeDown(nsize, time);
			case (3):
				return StartResizeRight(nsize, time);
			default:
				GD.PushError("Invalid side");
				return (new Vector2I(200, 200), new Vector2I(0,0));
		}
	}

	private int GetDistance()
	{
		switch (side)
		{
			case (0):
				return  Position.Y - CursorPosition.Y;
			case (1):
				return Position.X - CursorPosition.X;
			case (2):
				return CursorPosition.Y - Position.Y;
			case (3):
				return CursorPosition.X - Position.X;
			default:
				GD.PushError("Invalid side");
				return 0;

		}
	}


	public override void Start()
	{
		base.Start();
	}

	public override void Move()
	{
		const float MoveTime = 0.5f;
		const float margin = 0.1f;

		side = Lib.rand.Next(0, 4);
		TargetPosition = GetTargetPosition(side) - Size / 2;
		StartTransition(TargetPosition, MoveTime-margin); // to be sure that the transition is done before the next move
		windowPosition = TargetPosition;
		CursorPosition = Parent.CursorWindow.CenterPosition;

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);
		nsize = (int)(GetDistance() * 1.2f); // distance to the cursor window
		(Vector2I targetSize, Vector2I targetPosition) = CallResize(nsize,0f); // use CallResize just to get the target size and position
		IsResizing = false; //because we juste use CallResize to get the target size and position, we have to cancel the resizing
		ShowVisualCollision(targetSize, targetPosition);
		
		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;
		CallResize(nsize, ResizeTime);
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
