using Godot;
using System;

public partial class FlappyWindow : AttackWindow
{
	[Export] AttackWindow ConnectedWindow;
	public Vector2I ConnectedWindowPosition;
	public int nsize = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		ConnectedWindow.Timer.Stop();
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

		Vector2I targetPosition;
		Vector2I OtherTargetPosition;

		int targetX = 0;
		if(Lib.rand.Next(0,2)==0)
		{
			targetX = GameManager.ScreenSize.X - Size.X;
		}
		targetPosition = new Vector2I(targetX, 0);
		OtherTargetPosition = new Vector2I(targetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y);

		StartTransition(targetPosition, MoveTime - MarginTime);
		ConnectedWindow.StartTransition(OtherTargetPosition, MoveTime - MarginTime);
		windowPosition = targetPosition;
		ConnectedWindowPosition = OtherTargetPosition;

		Timer.WaitTime = MoveTime;
        base.Move();
    }

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);
		ConnectedWindow.StartShake(ShakeTime, 5);

		Vector2I targetSize;
		Vector2I targetPosition;
		
		Vector2I targetSize2;
		Vector2I targetPosition2;

		nsize = GameManager.ScreenSize.Y/2 - Size.Y - Parent.CursorWindow.Size.Y;
		(targetSize, targetPosition) = StartResizeDown(nsize, ShakeTime);
		(targetSize2, targetPosition2) = ConnectedWindow.StartResizeUp(nsize, ShakeTime);

		targetSize = new Vector2I(GameManager.ScreenSize.X, targetSize.Y);
		targetPosition = Vector2I.Zero;

		targetSize2 = new Vector2I(GameManager.ScreenSize.X, targetSize2.Y);
		targetPosition2 = new Vector2I(0, GameManager.ScreenSize.Y - targetSize2.Y);
		ShowVisualCollision(targetSize, targetPosition, ShakeTime);
		ConnectedWindow.ShowVisualCollision(targetSize2, targetPosition2, ShakeTime);

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float MoveTime = 0.3f;
		const float AttackDuration = 0.3f;
		int TargetX = 0;
		if(Position.X - Parent.CursorWindow.Position.X < 0)
		{
			TargetX = GameManager.ScreenSize.X - Size.X;
		}
		StartLinearTransition(new Vector2I(TargetX, 0), MoveTime);
		ConnectedWindow.StartLinearTransition(new Vector2I(TargetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y), MoveTime);
		HideVisualCollision();
		ConnectedWindow.HideVisualCollision();
		Timer.WaitTime = MoveTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		const float ResetTime = 1f;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, ResetTime);
		//StartTransition(windowPosition, ResetTime, reset: true);
		
		ConnectedWindow.resizeMode = TransitionMode.Exponential;
		ConnectedWindow.StartResize(ConnectedWindow.windowSize, ResetTime);
		//ConnectedWindow.StartTransition(ConnectedWindowPosition, ResetTime, reset: true);

		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
	}

}
