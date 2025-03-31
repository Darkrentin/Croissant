using Godot;
using System;
using System.Runtime.InteropServices;

public partial class CompressWindow : AttackWindow
{

	[Export] public AttackWindow ConnectedWindow;
	public Vector2I ConnectedWindowPosition;

	public int side;
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
		side = Lib.rand.Next(0, 2);
		const float MoveTime = 0.5f;
		const float MarginTime = 0.1f;
		if(side==0)
		{
			int targetX = Parent.CursorWindow.Position.X;
			Vector2I targetPosition = new Vector2I(targetX, 0);
			Vector2I OtherTargetPosition = new Vector2I(targetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y);

			StartTransition(targetPosition, MoveTime - MarginTime);
			ConnectedWindow.StartTransition(OtherTargetPosition, MoveTime - MarginTime);
			windowPosition = targetPosition;
			ConnectedWindowPosition = OtherTargetPosition;
		}
		else
		{
			int targetY = Parent.CursorWindow.Position.Y;
			Vector2I targetPosition = new Vector2I(0, targetY);
			Vector2I OtherTargetPosition = new Vector2I(GameManager.ScreenSize.X - ConnectedWindow.Size.X, targetY);
			StartTransition(targetPosition, MoveTime - MarginTime);
			ConnectedWindow.StartTransition(OtherTargetPosition, MoveTime - MarginTime);
			windowPosition = targetPosition;
			ConnectedWindowPosition = OtherTargetPosition;
		}

		Timer.WaitTime = MoveTime;
        base.Move();
   
    }

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);
		ConnectedWindow.StartShake(ShakeTime, 5);

		if(side==0)
		{
			nsize = GameManager.ScreenSize.Y/2 - Size.Y;
			(Vector2I targetSize, Vector2I targetPosition) = StartResizeDown(nsize, 0);
			IsResizing = false;
			(Vector2I targetSize2, Vector2I targetPosition2) = ConnectedWindow.StartResizeUp(nsize, 0);
			ConnectedWindow.IsResizing = false;

			ShowVisualCollision(targetSize, targetPosition);
			ConnectedWindow.ShowVisualCollision(targetSize2, targetPosition2);
		}
		else
		{
			nsize = GameManager.ScreenSize.X/2 - Size.X;
			(Vector2I targetSize, Vector2I targetPosition) = StartResizeRight(nsize, 0);
			IsResizing = false;
			(Vector2I targetSize2, Vector2I targetPosition2) = ConnectedWindow.StartResizeLeft(nsize, 0);
			ConnectedWindow.IsResizing = false;

			ShowVisualCollision(targetSize, targetPosition);
			ConnectedWindow.ShowVisualCollision(targetSize2, targetPosition2);
		}

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;

		if (side == 0)
		{
			StartResizeDown(nsize,ResizeTime);
			ConnectedWindow.StartResizeUp(nsize,ResizeTime);
		}
		else
		{
			StartResizeRight(nsize,ResizeTime);
			ConnectedWindow.StartResizeLeft(nsize,ResizeTime);
		}

		HideVisualCollision();
		ConnectedWindow.HideVisualCollision();
		Timer.WaitTime = ResizeTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		const float ResetTime = 1f;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, ResetTime);
		StartTransition(windowPosition, ResetTime, reset: true);
		
		ConnectedWindow.resizeMode = TransitionMode.Exponential;
		ConnectedWindow.StartResize(ConnectedWindow.windowSize, ResetTime);
		ConnectedWindow.StartTransition(ConnectedWindowPosition, ResetTime, reset: true);

		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
	}

}
