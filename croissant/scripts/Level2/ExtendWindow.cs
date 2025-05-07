using Godot;

public partial class ExtendWindow : AttackWindow
{
	public Vector2I TargetPosition;
	public Vector2I TargetSize;

	public override void _Ready()
	{

		base._Ready();
		VisualCollision.Color = Colors.Red;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void Move()
	{
		const float MoveTime = 0.5f;
		const float MarginTime = 0.1f;
		Vector2I margin = Level2.CursorWindow.Size / 2;
		TargetPosition = new Vector2I(Lib.rand.Next(CursorPosition.X - margin.X, CursorPosition.X + margin.X), Lib.rand.Next(CursorPosition.Y - margin.Y, CursorPosition.Y + margin.Y)) - Level2.CursorWindow.Size / 2;
		windowPosition = TargetPosition;
		windowSize = Size;
		StartTransition(TargetPosition, MoveTime - MarginTime);

		Timer.WaitTime = MoveTime;
		base.Move();
	}

	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5);

		TargetSize = new Vector2I(Size.X,Size.X) + new Vector2I(Lib.GetScreenSize(0.2f, 0.2f).X,Lib.GetScreenSize(0.2f, 0.2f).X);
		TargetSize-= TitleBarSize;
		//Vector2I targetPosition = Position + (Size / 2) - (TargetSize / 2);
		ShowVisualCollision(TargetSize, StartResize(TargetSize, -1f), ShakeTime);
		IsResizing = false;

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;
		StartResize(TargetSize, ResizeTime, KeepCenter: true);
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