using Godot;
using System;

[GlobalClass]
public partial class FloatWindow : Window
{
	[Export] public bool Shaking = false;
	[Export] public bool Draggable = true;
	[Export] public bool Minimizable = true;
	[Export] private TransitionMode transitionMode = TransitionMode.Linear;
	[Export] public TransitionMode resizeMode = TransitionMode.Linear;
	[Export] public float Smoothness = 5.0f;
	[Export] public bool CollisionDisabled = true;
	[Export] private PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("uid://q2tedokw1ckw");
	private int ShakeIntensity = 0;
	private Timer ShakeTimer;
	private Vector2I TargetPosition;
	private Vector2I StartPosition;
	private Vector2I TargetSize;
	private Vector2I StartSize;
	private float TransitionTime = 0.5f;
	private float ResizeTime = 0.5f;
	private float elapsedTimeTransition = 0;
	private float elapsedTimeResize = 0;
	private Vector2I SizeWithDecoration { get { return DisplayServer.WindowGetSizeWithDecorations(WindowId); } }
	private int WindowId { get { return GetWindowId(); } }
	public enum TransitionMode { Linear, Exponential, InverseExponential }
	public Vector2I BasePosition;
	public bool IsTransitioning = false;
	public bool IsResizing = false;
	public Vector2I CenterPosition { set { SetWindowPosition(value - Size / 2); } get { return Position + Size / 2; } }
	public int TitleBarHeight
	{
		get
		{
			return (int)(25 * GameManager.ScreenScale);//return GetSizeWithDecorations().Y - Size.Y;
		}
	}
	public Vector2I TitleBarSize { get { return new Vector2I(0, TitleBarHeight); } }
	public Rect2I WindowRect;
	public Action DeleteWindow;
	public string TransitionTag = "";

	public bool IsAlwaysOnTop = false;

	public override void _Ready()
	{
		if (!Borderless)
		{
			Unresizable = true;
			SharpCorners = true;
			Minimizable = false;
		}

		GetTree().AutoAcceptQuit = false;
		CloseRequested += OnClose;
		ShakeTimer = new Timer();
		AddChild(ShakeTimer);
		ShakeTimer.Timeout += StopShake;
		WindowRect = new Rect2I(Position, Size);
		KeepTitleVisible = false;
		GameManager.Windows.Add(this);
		IsAlwaysOnTop = AlwaysOnTop;
		Title = "";
	}
	public override void _Process(double delta)
	{
		if (WindowRect.Position != Position || WindowRect.Size != Size)
		{
			UpdateWindowRect();
		}

		if (!Draggable && HasFocus())
		{
			GameManager.FixWindow.GrabFocus();
		}

		if (!Minimizable && Mode == ModeEnum.Minimized)
		{
			Mode = ModeEnum.Windowed;
			GameManager.ClickSound.Play();
		}

		if (IsTransitioning || IsResizing)
			TransitionWindow(delta);

		if (Shaking)
			_ProcessShake();
	}

	private void UpdateWindowRect()
	{
		WindowRect.Position = Position;
		WindowRect.Size = Size;
	}

	// Start a transition to a target position with a given transition time
	// The transition mode can be set to linear or exponential
	public void StartTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		if (transitionTime <= 0)
		{
			return;
		}
		//Lib.Print("Transition Start !");
		StartPosition = Position;
		if (IsTransitioning && !reset)
			TargetPosition += targetPosition - StartPosition;
		else
			TargetPosition = targetPosition;
		TransitionTime = transitionTime;
		IsTransitioning = true;
		elapsedTimeTransition = 0;
		Smoothness = smoothness;
	}

	// Start a resize to a target size with a given resize time
	// The resize mode can be set to linear or exponential
	public Vector2I StartResize(Vector2I targetSize, float resizeTime, bool KeepCenter = true)
	{
		StartSize = Size;
		TargetSize = targetSize;
		ResizeTime = resizeTime;

		Vector2I newPosition = Position;
		if (KeepCenter)
		{
			Vector2I deltaSize = TargetSize - StartSize;
			Vector2I deltaPosition = new Vector2I(deltaSize.X / 2, deltaSize.Y / 2);
			newPosition = Position - deltaPosition;
			transitionMode = resizeMode;
		}

		if (resizeTime <= 0)
			return newPosition;

		IsResizing = true;
		elapsedTimeResize = 0;

		if (KeepCenter)
			StartTransition(newPosition, resizeTime);
		return newPosition;
	}

	public void StartLinearTransition(Vector2I targetPosition, float transitionTime, bool reset = false)
	{
		transitionMode = TransitionMode.Linear;
		StartTransition(targetPosition, transitionTime, Smoothness, reset);
	}

	public void StartExponentialTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		transitionMode = TransitionMode.Exponential;
		StartTransition(targetPosition, transitionTime, smoothness, reset);
	}

	public void StartInverseExponentialTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		transitionMode = TransitionMode.InverseExponential;
		StartTransition(targetPosition, transitionTime, smoothness, reset);
	}

	public void StartLinearResize(Vector2I targetSize, float resizeTime)
	{
		resizeMode = TransitionMode.Linear;
		StartResize(targetSize, resizeTime);
	}

	public void StartExponentialResize(Vector2I targetSize, float resizeTime)
	{
		resizeMode = TransitionMode.Exponential;
		StartResize(targetSize, resizeTime);
	}

	public void StartInverseExponentialResize(Vector2I targetSize, float resizeTime)
	{
		resizeMode = TransitionMode.InverseExponential;
		StartResize(targetSize, resizeTime);
	}

	public (Vector2I newSize, Vector2I newPosition) StartResizeDown(int nsize, float resizeTime)
	{
		Vector2I newSize = new Vector2I(Size.X, Size.Y + nsize);
		StartResize(newSize, resizeTime, false);
		return (newSize, Position);
	}

	public (Vector2I newSize, Vector2I newPosition) StartResizeUp(int nsize, float resizeTime)
	{
		Vector2I newSize = new Vector2I(Size.X, Size.Y + nsize);
		Vector2I newPosition = new Vector2I(Position.X, Position.Y - nsize);
		StartResize(newSize, resizeTime, false);
		StartTransition(newPosition, resizeTime);
		return (newSize, newPosition);
	}

	public (Vector2I newSize, Vector2I newPosition) StartResizeRight(int nsize, float resizeTime)
	{
		Vector2I newSize = new Vector2I(Size.X + nsize, Size.Y);
		StartResize(newSize, resizeTime, false);
		return (newSize, Position);
	}

	public (Vector2I newSize, Vector2I newPosition) StartResizeLeft(int nsize, float resizeTime)
	{
		Vector2I newSize = new Vector2I(Size.X + nsize, Size.Y);
		Vector2I newPosition = new Vector2I(Position.X - nsize, Position.Y);
		StartResize(newSize, resizeTime, false);
		StartTransition(newPosition, resizeTime);
		return (newSize, newPosition);
	}
	private void TransitionWindow(double delta)
	{
		float deltaFloat = (float)delta;

		if (IsTransitioning)
		{
			elapsedTimeTransition += deltaFloat;
			float progress = elapsedTimeTransition / TransitionTime;

			if (progress >= 1.0f)
			{
				SetWindowPosition(TargetPosition, true);
				IsTransitioning = false;
				TransitionFinished();
				return;
			}

			float adjustedProgress = CalculateProgress(progress, transitionMode);
			Vector2 lerpedPos = ((Vector2)StartPosition).Lerp(TargetPosition, adjustedProgress);
			SetWindowPosition((Vector2I)lerpedPos, true);
		}

		if (IsResizing)
		{
			elapsedTimeResize += deltaFloat;
			float progress = elapsedTimeResize / ResizeTime;

			if (progress >= 1.0f)
			{
				Size = TargetSize;
				IsResizing = false;
				ResizeFinished();
				return;
			}

			float adjustedProgress = CalculateProgress(progress, resizeMode);
			Vector2 lerpedSize = ((Vector2)StartSize).Lerp(TargetSize, adjustedProgress);
			Size = (Vector2I)lerpedSize;
		}
	}

	private float CalculateProgress(float progress, TransitionMode mode)
	{
		switch (mode)
		{
			case TransitionMode.Linear:
				return progress;

			case TransitionMode.Exponential:
				if (Smoothness > 0.01f)
					return (1.0f - Mathf.Exp(-progress * Smoothness)) / (1.0f - Mathf.Exp(-Smoothness));
				return progress;

			case TransitionMode.InverseExponential:
				if (Smoothness > 0.01f)
					return (Mathf.Exp((progress - 1.0f) * Smoothness) - Mathf.Exp(-Smoothness)) / (1.0f - Mathf.Exp(-Smoothness));
				return progress;

			default:
				return progress;
		}
	}

	//Set the window position and garranty that the window stay in the screen
	//return false if the window is out of the screen but the position is set to the nearest position
	//return true if the window is in the screen
	public bool SetWindowPosition(Vector2I newPosition, bool skipVerification = false)
	{
		if (skipVerification)
		{
			Position = newPosition;
			return true;
		}

		Vector2I clampedPosition = new Vector2I(
			Mathf.Clamp(newPosition.X, 0, GameManager.ScreenSize.X - Size.X),
			Mathf.Clamp(newPosition.Y, 0, GameManager.ScreenSize.Y - Size.Y)
		);

		Position = clampedPosition;
		return clampedPosition == newPosition;
	}

	public virtual void OnClose()
	{
		DeleteWindow();
	}

	public virtual void TransitionFinished() { }

	public virtual void ShakeFinished() { }

	public virtual void ResizeFinished() { }

	private Vector2I _shakeOffset = Vector2I.Zero;

	public void _ProcessShake()
	{
		if (!Shaking) return;

		if (IsTransitioning)
		{
			BasePosition = Position;
			return;
		}

		_shakeOffset.X = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
		_shakeOffset.Y = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
		Vector2I shakePosition = BasePosition + _shakeOffset;
		SetWindowPosition(shakePosition, true);
	}

	public void StartShake(float duration, int intensity)
	{
		BasePosition = Position;
		ShakeIntensity = intensity;
		if (duration != 0)
			ShakeTimer.Start(duration);
		Shaking = true;
	}

	public void StopShake()
	{
		Shaking = false;
		SetWindowPosition(BasePosition);
		ShakeTimer.Stop();
		/*
		CpuParticles2D explosion = ExplosionScene.Instantiate<CpuParticles2D>();
		explosion.Position = Position + Size / 2;

		GameManager.GameRoot.AddChild(explosion);
		////Lib.Print("EXPLOSION POSITION : " + explosion.Position);
		////Lib.Print("POSITION : " + Position + Size / 2);
		*/
		ShakeFinished();
	}

	public bool IsCollided(Rect2I other)
	{
		return WindowRect.Intersects(other);
	}

	public virtual void GrabWindowFocus()
	{
		GrabFocus();
	}
}
