using Godot;
using System;

public partial class LaserWindow : AttackWindow
{
	[Export] public AudioStreamPlayer AttackSound;
	protected int side;
	public Vector2I TargetPosition;
	private int nsize = 0;

	public override void _Ready()
	{
		base._Ready();
		VisualCollision.Color = Colors.Magenta;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}
	private Vector2I GetTargetPosition(int side)
	{
		int margin = Math.Max(Size.X, Size.Y);
		if (!RandomPosition)
			margin = (int)(Math.Max(Level2.CursorWindow.Size.X, Level2.CursorWindow.Size.Y) * 1.5f);

		////Lib.Print($"Side: {side}");
		Vector2I targetPos;
		switch (side)
		{
			case 3: // Left
				targetPos = new Vector2I(Lib.rand.Next(0, Math.Max(0, CursorPosition.X - margin)), CursorPosition.Y);
				break;
			case 0: // Bottom
				targetPos = new Vector2I(CursorPosition.X, Lib.rand.Next(Math.Min(GameManager.ScreenSize.Y, CursorPosition.Y + margin), GameManager.ScreenSize.Y));
				break;
			case 1: // Right
				targetPos = new Vector2I(Lib.rand.Next(Math.Min(CursorPosition.X + margin, GameManager.ScreenSize.X), GameManager.ScreenSize.X), CursorPosition.Y);
				break;
			case 2: // Top
				targetPos = new Vector2I(CursorPosition.X, Lib.rand.Next(0, Math.Max(0, CursorPosition.Y - margin)));
				break;
			default:
				GD.PushError("Invalid side");
				targetPos = new Vector2I(0, 0);
				break;
		}
		
		// Ensure the target position stays within screen bounds
		return ClampToScreen(targetPos);
	}

	private (Vector2I newSize, Vector2I newPosition) CallResize(int nsize, float time)
	{
		switch (side)
		{
			case 0:
				return StartResizeUp(nsize, time);
			case 1:
				return StartResizeLeft(nsize, time);
			case 2:
				return StartResizeDown(nsize, time);
			case 3:
				return StartResizeRight(nsize, time);
			default:
				GD.PushError("Invalid side");
				return (new Vector2I(200, 200), new Vector2I(0, 0));
		}
	}
	private int GetDistance()
	{
		Vector2I targetCenter = TargetPosition + Size / 2;
		switch (side)
		{
			case 0:
				return targetCenter.Y - CursorPosition.Y;
			case 1:
				return targetCenter.X - CursorPosition.X;
			case 2:
				return CursorPosition.Y - targetCenter.Y;
			case 3:
				return CursorPosition.X - targetCenter.X;
			default:
				GD.PushError("Invalid side");
				return 0;
		}
	}

	public override void Start()
	{
		base.Start();
	}	public override void Move()
	{
		const float MoveTime = 0.5f;
		const float margin = 0.1f;

		side = Lib.rand.Next(0, 4);
		TargetPosition = GetTargetPosition(side) - Size / 2;
		StartTransition(TargetPosition, MoveTime - margin);
		windowPosition = TargetPosition;

		// Calculate the distance to the cursor window during Move phase to prevent player from resizing after calculation
		if (!RandomPosition)
		{
			nsize = (int)(GetDistance() * 1.2f); // distance to the cursor window
		}		else
		{
			// For random position, calculate exact distance to screen edge in resize direction
			Vector2I targetCenter = TargetPosition + Size / 2;
			Vector2I currentSize = Size;
			
			switch (side)
			{
				case 0: // Resize Up (towards top) - expand upward
					nsize = targetCenter.Y - currentSize.Y / 2;
					break;
				case 1: // Resize Left (towards left) - expand leftward  
					nsize = targetCenter.X - currentSize.X / 2;
					break;
				case 2: // Resize Down (towards bottom) - expand downward
					nsize = GameManager.ScreenSize.Y - targetCenter.Y - currentSize.Y / 2;
					break;
				case 3: // Resize Right (towards right) - expand rightward
					nsize = GameManager.ScreenSize.X - targetCenter.X - currentSize.X / 2;
					break;
				default:
					nsize = Math.Min(GameManager.ScreenSize.X, GameManager.ScreenSize.Y) / 2;
					break;
			}
			// Ensure nsize is positive and covers the distance to screen edge
			nsize = Math.Max(10, nsize); // Minimum size to ensure laser is visible
		}

		Timer.WaitTime = MoveTime;
		base.Move();
	}
	public override void Prevent()
	{
		const float ShakeTime = 1f;
		StartShake(ShakeTime, 5); //FIND WHY THE WINDOWS DISEAPPEAR WHEN I DON'T USE THE SHAKE !
		
		// nsize is now calculated during Move phase to prevent player from exploiting movement after calculation
		(Vector2I targetSize, Vector2I targetPosition) = CallResize(nsize, -1f);
		IsResizing = false;
		ShowVisualCollision(targetSize, targetPosition, ShakeTime);

		Timer.WaitTime = ShakeTime;
		base.Prevent();
	}

	public override void Attack()
	{
		AttackSound.Play();
		const float ResizeTime = 0.2f;
		const float AttackDuration = 0.3f;
		CallResize(nsize, ResizeTime);


		Timer.WaitTime = ResizeTime + AttackDuration;
		base.Attack();
	}

	public override void Reload()
	{
		const float ResetTime = 1f;
		resizeMode = TransitionMode.Exponential;
		StartResize(windowSize, ResetTime);
		StartTransition(windowPosition, ResetTime, reset: true);
		HideVisualCollision();

		Timer.WaitTime = Lib.GetRandomNormal(0.5f, 3.0f); // time to wait before restarting
		base.Reload();
	}
}
