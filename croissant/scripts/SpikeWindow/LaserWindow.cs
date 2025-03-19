using Godot;
using System;

public partial class LaserWindow : FloatWindow
{

	private Level2 Parent;
	public Vector2I windowSize;
	public Vector2I windowPosition;
	[Export] public Timer Timer;

	public CollisionShape2D collision;
	public RigidBody2D body;

	public bool Attacking = false;

	private int side;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();

		Parent = GetParent<Level2>();
		//Position = Lib.GetRandomPositionOutsideScreen(side);
		
		windowSize = Size;
		Start();

	}

	public void Start()
	{
		Timer.WaitTime = 0.5f;
		Timer.Timeout+=StartAttack;
		Timer.Start();

		side = Lib.rand.Next(0, 4);
		Vector2I TargetPosition = GetTargetPosition(side) - Size/2;
		StartTransition(TargetPosition, 0.5f);
		windowPosition = TargetPosition;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		if(Attacking && IsCollided(Parent.CursorWindow))
		{
			GD.Print("Collided");
			Parent.CursorWindow.TakeDamage();

		}
	}

	private Vector2I GetTargetPosition(int side)
	{
		const int margin = 150;
		GD.Print(side);
		switch(side)
		{
			case(3):
				return new Vector2I(Lib.rand.Next(0, Parent.CursorWindow.CenterPosition.X - margin), Parent.CursorWindow.CenterPosition.Y);
			case(0):
				return new Vector2I(Parent.CursorWindow.CenterPosition.X, Lib.rand.Next(Parent.CursorWindow.CenterPosition.Y + margin, GameManager.ScreenSize.Y));
			case(1):
				return new Vector2I(Lib.rand.Next(Parent.CursorWindow.CenterPosition.X + margin, GameManager.ScreenSize.X), Parent.CursorWindow.CenterPosition.Y);
			case(2):
				return new Vector2I(Parent.CursorWindow.CenterPosition.X, Lib.rand.Next(0, Parent.CursorWindow.CenterPosition.Y - margin));
			default:
				GD.PushError("Invalid side");
				return new Vector2I(0,0);
				
		}
	}

	private void CallResize(int nsize, float time)
	{
		switch(side)
		{
			case(0):
				StartResizeUp(nsize, time);
				break;
			case(1):
				StartResizeLeft(nsize, time);
				break;
			case(2):
				StartResizeDown(nsize, time);
				break;
			case(3):
				StartResizeRight(nsize, time);
				break;
			default:
				GD.PushError("Invalid side");
				break;
		}
	}

	public void StartAttack()
	{
		StartShake(0.3f, 5);
		Timer.WaitTime = 0.3f;
		Timer.Timeout-=StartAttack;
		Timer.Timeout+=ShakeEnd;
		Timer.Start();

	}

	public void TimeoutResize()
	{
		Attacking = false;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, 1f);
		StartTransition(windowPosition,1f,reset:true);
		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f);
		Timer.Timeout-=TimeoutResize;
		Timer.Timeout+=ReStart;
		Timer.Start();
;
	}

    public void ShakeEnd()
    {
		CallResize(1000, 0.2f);
		Attacking = true;
		Timer.WaitTime = 0.5f;
		Timer.Timeout-=ShakeEnd;
		Timer.Timeout+=TimeoutResize;
    }

	public void ReStart()
	{
		Timer.Timeout-=ReStart;
		Start();
	}



}
