using Godot;

public partial class FlappyWindow : AttackWindow
{
	[Export] AttackWindow ConnectedWindow;
	public Vector2I ConnectedWindowPosition;
	public int nsizeA = 0;
	public int nsizeB = 0;

	public override void _Ready()
	{
		base._Ready();
		ConnectedWindow.Timer.Stop();
		int r = Lib.rand.Next(0, 3);
		switch (r)
		{
			case 0:
				ConnectedWindow.Visible = false;
				ConnectedWindow.CollisionDisabled = true;
				break;
			case 1:
				Visible = false;
				CollisionDisabled = true;
				break;
		}
	}

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
		if (Lib.rand.Next(0, 2) == 0)
			targetX = GameManager.ScreenSize.X - Size.X;
		if (Visible)
		{
			targetPosition = new Vector2I(targetX, 0);
			StartTransition(targetPosition, MoveTime - MarginTime);
			windowPosition = targetPosition;
		}
		if (ConnectedWindow.Visible)
		{
			OtherTargetPosition = new Vector2I(targetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y);
			ConnectedWindow.StartTransition(OtherTargetPosition, MoveTime - MarginTime);
			ConnectedWindowPosition = OtherTargetPosition;
		}

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		const int Margin = 150;
		const float SizeMargin = 1.5f;

		Vector2I targetSize;
		Vector2I targetPosition;

		Vector2I targetSize2;
		Vector2I targetPosition2;

		int targetY = Lib.rand.Next(Size.Y + Margin, GameManager.ScreenSize.Y - Size.Y - Margin);
		nsizeA = targetY - Level2.CursorWindow.Size.Y - (int)(Size.Y * SizeMargin);
		nsizeB = GameManager.ScreenSize.Y - targetY - Level2.CursorWindow.Size.Y - (int)(ConnectedWindow.Size.Y * SizeMargin);

		if (Visible)
		{
			StartShake(ShakeTime, 5);
			(targetSize, targetPosition) = StartResizeDown(nsizeA, ShakeTime);

			targetSize = new Vector2I(GameManager.ScreenSize.X, targetSize.Y);
			targetPosition = Vector2I.Zero;

			ShowVisualCollision(targetSize, targetPosition, Colors.Magenta);
		}

		if (ConnectedWindow.Visible)
		{
			ConnectedWindow.StartShake(ShakeTime, 5);
			(targetSize2, targetPosition2) = ConnectedWindow.StartResizeUp(nsizeB, ShakeTime);

			targetSize2 = new Vector2I(GameManager.ScreenSize.X, targetSize2.Y);
			targetPosition2 = new Vector2I(0, GameManager.ScreenSize.Y - targetSize2.Y);

			ConnectedWindow.ShowVisualCollision(targetSize2, targetPosition2, Colors.Magenta);
		}

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float MoveTime = 0.3f;
		const float AttackDuration = 0.3f;
		int TargetX = 0;
		int ConnectedWindowTargetX = 0;
		if (Position.X - Level2.CursorWindow.Position.X < 0)
			TargetX = GameManager.ScreenSize.X - Size.X;
		if (ConnectedWindow.Position.X - Level2.CursorWindow.Position.X < 0)
			ConnectedWindowTargetX = GameManager.ScreenSize.X - ConnectedWindow.Size.X;
		if (Visible)
		{
			StartLinearTransition(new Vector2I(TargetX, 0), MoveTime);
			HideVisualCollision();
		}

		if (ConnectedWindow.Visible)
		{
			ConnectedWindow.StartLinearTransition(new Vector2I(ConnectedWindowTargetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y), MoveTime);
			ConnectedWindow.HideVisualCollision();
		}

		Timer.WaitTime = MoveTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		const float ResetTime = 1f;
		if (Visible)
		{
			resizeMode = TransitionMode.Exponential;
			StartResize(windowSize, ResetTime);
			StartTransition(new Vector2I(Position.X, 0), ResetTime, reset: true);
		}

		if (ConnectedWindow.Visible)
		{
			ConnectedWindow.resizeMode = TransitionMode.Exponential;
			ConnectedWindow.StartResize(ConnectedWindow.windowSize, ResetTime);
			ConnectedWindow.StartTransition(new Vector2I(ConnectedWindow.Position.X, GameManager.ScreenSize.Y - ConnectedWindow.windowSize.Y), ResetTime, reset: true);
		}

		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		if (Lives <= 0)
			ConnectedWindow.Delete();
		base.Reload();
	}
}