using Godot;

public partial class CompressWindow : AttackWindow
{
	[Export] public AudioStreamPlayer AttackSound;
	[Export] public AttackWindow ConnectedWindow;
	public Vector2I ConnectedWindowPosition;
	public int _Mode = -1;
	public int side;
	public int nsize = 0;

	public override void _Ready()
	{
		base._Ready();
		VisualCollision.Color = Colors.Yellow;
		ConnectedWindow.VisualCollision.Color = Colors.Yellow;
		ConnectedWindow.Timer.Stop();
		ConnectedWindow.Lives = Lives;

		ConnectedWindow.Position = Lib.GetRandomPositionOutsideScreen(-1, Size.X * 2);

		ConnectedWindow.ParentWave = ParentWave;
		Wave.NbOfEnemies++;

		RemoveChild(ConnectedWindow);
		GetParent().AddChild(ConnectedWindow);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	public override void Move()
	{
		if (_Mode == -1)
		{
			side = Lib.rand.Next(0, 2);
		}
		else
		{
			side = _Mode;
		}
		const float MoveTime = 0.5f;
		const float MarginTime = 0.1f;

		Vector2I targetPosition;
		Vector2I OtherTargetPosition;

		if (side == 0)
		{
			int targetX = CursorPosition.X;
			targetPosition = ClampToScreen(new Vector2I(targetX, 0));
			OtherTargetPosition = ClampToScreen(new Vector2I(targetX, GameManager.ScreenSize.Y - ConnectedWindow.Size.Y), ConnectedWindow.Size);
		}
		else
		{
			int targetY = CursorPosition.Y;
			targetPosition = ClampToScreen(new Vector2I(0, targetY));
			OtherTargetPosition = ClampToScreen(new Vector2I(GameManager.ScreenSize.X - ConnectedWindow.Size.X, targetY), ConnectedWindow.Size);
		}

		StartTransition(targetPosition, MoveTime - MarginTime);
		ConnectedWindow.StartTransition(OtherTargetPosition, MoveTime - MarginTime);
		windowPosition = targetPosition;
		ConnectedWindowPosition = OtherTargetPosition;


		Timer.WaitTime = MoveTime;
		base.Move();
		ConnectedWindow.CurrentPhase = Phase.Move;
	}

	public override void Prevent()
	{
		Position = windowPosition;
		ConnectedWindow.Position = ConnectedWindowPosition;

		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);
		ConnectedWindow.StartShake(ShakeTime, 5);

		Vector2I targetSize;
		Vector2I targetPosition;

		Vector2I targetSize2;
		Vector2I targetPosition2;

		if (side == 0)
		{
			nsize = GameManager.ScreenSize.Y / 2 - (Size.Y + (TitleBarHeight / 2));
			(targetSize, targetPosition) = StartResizeDown(nsize, -1f);
			(targetSize2, targetPosition2) = ConnectedWindow.StartResizeUp(nsize, -1f);
		}
		else
		{
			nsize = GameManager.ScreenSize.X / 2 - Size.X;
			(targetSize, targetPosition) = StartResizeRight(nsize, -1f);
			(targetSize2, targetPosition2) = ConnectedWindow.StartResizeLeft(nsize, -1f);
		}

		IsResizing = false;
		ConnectedWindow.IsResizing = false;

		ShowVisualCollision(targetSize, targetPosition, ShakeTime);
		ConnectedWindow.ShowVisualCollision(targetSize2, targetPosition2, ShakeTime);

		Timer.WaitTime = ShakeTime;
		base.Prevent();
		ConnectedWindow.CurrentPhase = Phase.Prevent;
	}

	public override void Attack()
	{
		AttackSound.Play();
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;

		if (side == 0)
		{
			StartResizeDown(nsize, ResizeTime);
			ConnectedWindow.StartResizeUp(nsize, ResizeTime);
		}
		else
		{
			StartResizeRight(nsize, ResizeTime);
			ConnectedWindow.StartResizeLeft(nsize, ResizeTime);
		}


		Timer.WaitTime = ResizeTime + AttackDuration;
		base.Attack();
		ConnectedWindow.CurrentPhase = Phase.Attack;
	}

	public override void Reload()
	{

		HideVisualCollision();
		ConnectedWindow.HideVisualCollision();

		const float ResetTime = 1f;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, ResetTime);
		StartTransition(windowPosition, ResetTime, reset: true);

		ConnectedWindow.resizeMode = TransitionMode.Exponential;
		ConnectedWindow.StartResize(ConnectedWindow.windowSize, ResetTime);
		ConnectedWindow.StartTransition(ConnectedWindowPosition, ResetTime, reset: true);

		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
		ConnectedWindow.CurrentPhase = Phase.Reload;
	}

	public override void Delete()
	{
		// Assurez-vous que la fenêtre connectée est supprimée correctement
		if (ConnectedWindow != null && IsInstanceValid(ConnectedWindow))
		{
			ConnectedWindow.Delete();
			ConnectedWindow.QueueFree();
		}
		base.Delete();
	}
}
