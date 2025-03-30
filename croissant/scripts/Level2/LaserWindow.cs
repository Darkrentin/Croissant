using Godot;
using System;

public partial class LaserWindow : AttackWindow
{
	protected int side;

	public Vector2I TargetPosition;
	private int nsize = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Start();

	}

	public void Start()
	{
		Timer.WaitTime = 0.5f;
		Timer.Timeout += StartAttack;
		Timer.Start();

		side = Lib.rand.Next(0, 4);
		TargetPosition = GetTargetPosition(side) - Size / 2;
		StartTransition(TargetPosition, 0.5f);
		windowPosition = TargetPosition;
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

	private void CallResize(int nsize, float time)
	{
		switch (side)
		{
			case (0):
				StartResizeUp(nsize, time);
				break;
			case (1):
				StartResizeLeft(nsize, time);
				break;
			case (2):
				StartResizeDown(nsize, time);
				break;
			case (3):
				StartResizeRight(nsize, time);
				break;
			default:
				GD.PushError("Invalid side");
				break;
		}
	}

	private (Vector2I,Vector2I) GetTargetSizeAndPosition(int nsize)
	{
		switch (side)
		{
			case (0):
				return (new Vector2I(Size.X, Size.Y + nsize), new Vector2I(TargetPosition.X, TargetPosition.Y - nsize));
			case (1):
				return (new Vector2I(Size.X + nsize, Size.Y), new Vector2I(TargetPosition.X - nsize, TargetPosition.Y));
			case (2):
				return (new Vector2I(Size.X, Size.Y + nsize), new Vector2I(TargetPosition.X, TargetPosition.Y));
			case (3):
				return (new Vector2I(Size.X + nsize, Size.Y), new Vector2I(TargetPosition.X, TargetPosition.Y));
			default:
				GD.PushError("Invalid side");
				return (new Vector2I(0, 0), new Vector2I(142, 142));

		}
	}

	private int GetDistance()
	{
		switch (side)
		{
			case (0):
			Lib.Print($"Distance: {Parent.CursorWindow.Position.Y - Position.Y}");
				return  Position.Y - Parent.CursorWindow.Position.Y;
			case (1):
				Lib.Print($"Distance: {Position.X - Parent.CursorWindow.Position.X}");
				return Position.X - Parent.CursorWindow.Position.X;
			case (2):
				Lib.Print($"Distance: {Position.Y - Parent.CursorWindow.Position.Y}");
				return Parent.CursorWindow.Position.Y - Position.Y;
			case (3):
				Lib.Print($"Distance: {Position.X - Parent.CursorWindow.Position.X}");
				return Parent.CursorWindow.Position.X - Position.X;
			default:
				GD.PushError("Invalid side");
				return 0;

		}
	}

	public void StartAttack()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);
		(Vector2I targetSize, Vector2I targetPosition) = GetTargetSizeAndPosition(nsize);
		ShowVisualCollision(targetSize, targetPosition);
		Timer.WaitTime = ShakeTime;
		Timer.Timeout -= StartAttack;
		Timer.Timeout += ShakeEnd;
		Timer.Start();

	}

	public void TimeoutResize()
	{
		Attacking = false;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, 1f);
		StartTransition(windowPosition, 1f, reset: true);
		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f);
		Timer.Timeout -= TimeoutResize;
		Timer.Timeout += ReStart;
		Timer.Start();
	}

	public void ShakeEnd()
	{
		nsize = GetDistance();
		CallResize(nsize, 0.2f);
		HideVisualCollision();
		Attacking = true;
		Timer.WaitTime = 0.5f;
		Timer.Timeout -= ShakeEnd;
		Timer.Timeout += TimeoutResize;
	}

	public void ReStart()
	{
		Timer.Timeout -= ReStart;
		Start();
	}

	//NOT WORKING
	public override void WindowCollided(FloatWindow window)
	{
		if (window is CursorWindow w && Attacking && !w.Shaking)
		{
			Lib.Print("Collided");
			w.TakeDamage();
		}
	}


}
