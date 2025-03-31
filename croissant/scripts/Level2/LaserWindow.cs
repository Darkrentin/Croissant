using Godot;
using System;
using System.Runtime.CompilerServices;

public partial class LaserWindow : AttackWindow
{
	protected int side;

	public Vector2I TargetPosition;
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
		int margin = 0;
		if(!Random)
		{
			margin = Math.Max(Parent.CursorWindow.Size.X, Parent.CursorWindow.Size.Y);
		}
		
		Lib.Print($"Side: {side}");
		switch (side)
		{
			case (3):
				return new Vector2I(Lib.rand.Next(0, (CursorPosition.X - margin)%GameManager.ScreenSize.X), CursorPosition.Y);
			case (0):
				return new Vector2I(CursorPosition.X, Lib.rand.Next((CursorPosition.X + margin)%GameManager.ScreenSize.Y, GameManager.ScreenSize.Y));
			case (1):
				return new Vector2I(Lib.rand.Next((CursorPosition.X + margin)%GameManager.ScreenSize.X, GameManager.ScreenSize.X), CursorPosition.Y);
			case (2):
				return new Vector2I(CursorPosition.X, Lib.rand.Next(0, (CursorPosition.X - margin)%GameManager.ScreenSize.Y));
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
				return  CenterPosition.Y - CursorPosition.Y;
			case (1):
				return CenterPosition.X - CursorPosition.X;
			case (2):
				return CursorPosition.Y - CenterPosition.Y;
			case (3):
				return CursorPosition.X - CenterPosition.X;
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

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		//StartShake(ShakeTime, 5); //FIND WHY THE WINDOWS DISEAPPEAR WHEN I DON'T USE THE SHAKE !
		if(!Random)
		{
			nsize = (int)(GetDistance() * 1.2f); // distance to the cursor window
		}
		else
		{
			nsize = Math.Max(GameManager.ScreenSize.X, GameManager.ScreenSize.Y);
		}
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
