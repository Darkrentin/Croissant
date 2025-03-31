using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;


public partial class FloatWindow : Window
{

	public enum TransitionMode
	{
		Linear,
		Exponential,
		InverseExponential
	}

	//additionnal properties
	[Export] public bool Draggable = true;
	[Export] public bool Minimizable = true;

	[Export] public bool Shaking = false;
	public Timer ShakeTimer;

	public int ShakeIntensity = 0;
	public Vector2I BasePosition;


	//transition properties
	[Export] public TransitionMode transitionMode = TransitionMode.Linear;
	[Export] public TransitionMode resizeMode = TransitionMode.Linear;
	[Export] public float Smoothness = 5.0f;

	private Vector2I TargetPosition;
	private Vector2I StartPosition;

	private Vector2I TargetSize;
	private Vector2I StartSize;

	private float TransitionTime = 0.5f;
	private float ResizeTime = 0.5f;
	private float elapsedTimeTransition = 0;
	private float elapsedTimeResize = 0;
	public bool IsTransitioning = false;
	public bool IsResizing = false;

	public Vector2I CenterPosition {
		set { SetWindowPosition(value-Size / 2); }
		get { return Position + Size / 2; }
	}

	public Vector2I SizeWithDecoration
	{
		get { return DisplayServer.WindowGetSizeWithDecorations(WindowId); }
	}

	[Export] public bool CollisionDisabled = true;

	public List<FloatWindow> CollidedWindows = new List<FloatWindow>();

	[Export] public PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("uid://q2tedokw1ckw");

	public int WindowId
	{
		get { return GetWindowId(); }
	}

	public int titleBarHeight
	{
		get { return (DisplayServer.WindowGetSizeWithDecorations(WindowId) - DisplayServer.WindowGetSize(WindowId)).Y;}
	}

	public Vector2I titleBarSize
	{
		get { return new Vector2I(0,titleBarHeight);}

	}


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().AutoAcceptQuit = false; // Prevent the game from closing when the window is closed
		CloseRequested += OnClose; // Connect the close event to the OnClose function to catch the close event
		ShakeTimer = new Timer();
		AddChild(ShakeTimer);
		ShakeTimer.Timeout += () => { StopShake(); };
		GameManager.Windows.Add(this);
		Title = "Window Added";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockAction(); // Block action made by the player depending on the window properties
		TransitionWindow(delta); //Compute the transition of the window
		ProcessShake();
		CheckCollision();
		// Example
		/*
		if(Input.IsActionJustPressed("U"))
		{
			StartResizeUp(1000, 0.1f);
		}
		if(Input.IsActionJustPressed("D"))
		{
			StartResizeDown(1000, 0.1f);
		}
		if(Input.IsActionJustPressed("L"))
		{
			StartResizeLeft(1000, 0.1f);
		}
		if(Input.IsActionJustPressed("R"))
		{
			StartResizeRight(1000, 0.1f);
		}
		*/
	}

	// Block action made by the player depending on the window properties
	private void BlockAction()
	{
		//if the window is not draggable and has the focus, we give the focus to the FixWindow to cancel the drag
		if (!Draggable)
		{
			if (HasFocus())
			{
				GameManager.FixWindow.GrabFocus();
			}
		}
		//if the window is minimized, we change the mode to windowed to cancel the minimization
		if (!Minimizable)
		{
			if (Mode == ModeEnum.Minimized)
			{
				Mode = ModeEnum.Windowed;
			}
		}
	}

	// Start a transition to a target position with a given transition time
	// The transition mode can be set to linear or exponential
	public void StartTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		StartPosition = Position;
		if (IsTransitioning && !reset)
		{
			TargetPosition += targetPosition - StartPosition;
		}
		else
		{
			TargetPosition = targetPosition;
		}
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
		IsResizing = true;
		elapsedTimeResize = 0;

		if (KeepCenter)
		{
			Vector2I deltaSize = TargetSize - StartSize;
			Vector2I deltaPosition = new Vector2I(deltaSize.X / 2, deltaSize.Y / 2);
			Vector2I newPosition = Position - deltaPosition;
			transitionMode = resizeMode;
			StartTransition(newPosition, resizeTime);
			return newPosition;
		}
		return Position;

	}

	// Start a linear transition to a target position
	public void StartLinearTransition(Vector2I targetPosition, float transitionTime, bool reset = false)
	{
		transitionMode = TransitionMode.Linear;
		StartTransition(targetPosition, transitionTime, Smoothness, reset);
	}

	// Start an exponential transition to a target position
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




	// Start a linear resize to a target size
	public void StartLinearResize(Vector2I targetSize, float resizeTime)
	{
		resizeMode = TransitionMode.Linear;
		StartResize(targetSize, resizeTime);
	}

	// Start an exponential resize to a target sizes
	public void StartExponentialResize(Vector2I targetSize, float resizeTime)
	{
		resizeMode = TransitionMode.Exponential;
		StartResize(targetSize, resizeTime);
	}

	// Start an inverse exponential resize to a target size
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




	// Compute the transition of the window
	private void TransitionWindow(double delta)
	{
		if (IsTransitioning)
		{
			// Check if the transition is still in progress
			if (elapsedTimeTransition < TransitionTime)
			{
				// Increment the elapsed time
				elapsedTimeTransition += (float)delta;
				Vector2I newPosition;

				// Normalized progress from 0.0 to 1.0
				float progress = Mathf.Clamp(elapsedTimeTransition / TransitionTime, 0f, 1f);

				// Compute the new position based on the transition mode
				switch (transitionMode)
				{
					case TransitionMode.Linear:
						// Linear interpolation - moves at constant speed
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, progress);
						//Lib.Print($"transition Time: {elapsedTimeTransition} Progress: {progress}");
						break;

					case TransitionMode.Exponential:
						// Exponential easing function that guarantees completion in TransitionTime
						// Uses the formula 1 - exp(-t * k) / (1 - exp(-k)) where k controls the curve shape
						float k = Smoothness; // Adjust the curve steepness
						float expProgress;

						if (k > 0.01f)
						{
							expProgress = (1.0f - Mathf.Exp(-progress * k)) / (1.0f - Mathf.Exp(-k));
						}
						else
						{
							// Fallback to linear for very small speed values
							expProgress = progress;
						}
						//Lib.Print($"Transtition Time: {elapsedTimeTransition} Progress: {progress} ExpProgress: {expProgress}");

						// Compute the new position based on the exponential easing function
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, expProgress);
						break;
					
					case TransitionMode.InverseExponential:
                        // Inverse exponential easing - starts fast and slows down
                        float ik = Smoothness;
                        float invExpProgress;
                        
                        if (ik > 0.01f)
                        {
                            // Use 1-(1-t)^2 for inverse exponential effect
                            invExpProgress = 1.0f - (Mathf.Exp((progress - 1.0f) * ik) - Mathf.Exp(-ik)) / (1.0f - Mathf.Exp(-ik));
                        }
                        else
                        {
                            // Fallback to linear for very small values
                            invExpProgress = progress;
                        }
                        
                        newPosition = (Vector2I)((Godot.Vector2)TargetPosition).Lerp(StartPosition,invExpProgress);
                        break;

					default:
						GD.PushWarning("Invalid transition mode");
						newPosition = Position;
						break;
				}

				SetWindowPosition(newPosition, true);
			}
			else
			{
				// Ensure we end exactly at the target position
				SetWindowPosition(TargetPosition, true);
				IsTransitioning = false;
				TransitionFinished();

			}
		}
		if (IsResizing)
		{
			// Check if the resize is still in progress
			if (elapsedTimeResize < ResizeTime)
			{
				// Increment the elapsed time
				elapsedTimeResize += (float)delta;
				Vector2I newSize;

				// Normalized progress from 0.0 to 1.0
				float progress = Mathf.Clamp(elapsedTimeResize / ResizeTime, 0f, 1f);

				// Compute the new size based on the resize mode
				switch (resizeMode)
				{
					case TransitionMode.Linear:
						// Linear interpolation - resizes at constant speed
						newSize = (Vector2I)((Godot.Vector2)StartSize).Lerp(TargetSize, progress);
						//Lib.Print($"resize Time: {elapsedTimeResize} Progress: {progress}");
						break;
					case TransitionMode.Exponential:
						// Exponential easing function that guarantees completion in ResizeTime
						// Uses the formula 1 - exp(-t * k) / (1 - exp(-k)) where k controls the curve shape
						float k = Smoothness;
						float expProgress;
						if (k > 0.01f)
						{
							expProgress = (1.0f - Mathf.Exp(-progress * k)) / (1.0f - Mathf.Exp(-k));
						}
						else
						{
							expProgress = progress;
						}

						// Compute the new size based on the exponential easing function
						newSize = (Vector2I)((Godot.Vector2)StartSize).Lerp(TargetSize, expProgress);
						//Lib.Print($"Resize Time: {elapsedTimeResize} Progress: {progress} ExpProgress: {expProgress}");
						break;
					
					case TransitionMode.InverseExponential:
                        // Inverse exponential easing - starts fast and slows down
                        float ik = Smoothness;
                        float invExpProgress;
                        
                        if (ik > 0.01f)
                        {
                            // Use inverse exponential curve
                            invExpProgress = 1.0f - (Mathf.Exp((progress - 1.0f) * ik) - Mathf.Exp(-ik)) / (1.0f - Mathf.Exp(-ik));
                        }
                        else
                        {
                            // Fallback to linear for very small values
                            invExpProgress = progress;
                        }
                        
                        newSize = (Vector2I)((Godot.Vector2)StartSize).Lerp(TargetSize, invExpProgress);
                        break;

					default:
						GD.PushWarning("Invalid resize mode");
						newSize = Size;
						break;
				}
				Size = newSize;
			}
			else
			{
				Size = TargetSize;
				IsResizing = false;
				ResizeFinished();
			}
		}
	}

	//Set the window position and garranty that the window stay in the screen
	//return false if the window is out of the screen but the position is set to the nearest position
	//return true if the window is in the screen
	public bool SetWindowPosition(Vector2I newPosition, bool SkipVerification = false)
	{
		if (SkipVerification)
		{
			Position = newPosition;
			return true;
		}
		int x = Position.X;
		int y = Position.Y;
		bool returnValue = true;

		if (newPosition.X < 0)
		{
			x = 0;
			returnValue = false;
		}
		else if (newPosition.X > GameManager.ScreenSize.X - Size.X)
		{
			x = GameManager.ScreenSize.X - Size.X;
			returnValue = false;
		}
		else
		{
			x = newPosition.X;
		}

		if (newPosition.Y < 0)
		{
			y = 0;
			returnValue = false;
		}
		else if (newPosition.Y > GameManager.ScreenSize.Y - Size.Y)
		{
			y = GameManager.ScreenSize.Y - Size.Y;
			returnValue = false;
		}
		else
		{
			y = newPosition.Y;
		}

		Position = new Vector2I(x, y);

		return returnValue;
	}

	// Called when the window is closed
	public virtual void OnClose()
	{
		//Lib.Print("Window Closed");
	}

	public virtual void TransitionFinished()
	{
		//Lib.Print("Transition Finished");
	}

	public virtual void ShakeFinished()
	{
		//Lib.Print("Shake Finished");
	}

	public virtual void ResizeFinished()
	{
		//Lib.Print("Resize Finished");
	}

	public void ProcessShake()
	{
		if (Shaking)
		{
			if (IsTransitioning)
			{
				BasePosition = Position;
				return;
			}

			int offsetX = (int)Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
			int offsetY = (int)Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);

			Vector2I ShakePosition = BasePosition + new Vector2I(offsetX, offsetY);

			SetWindowPosition(ShakePosition);
		}
	}

	public void StartShake(float duration, int intensity)
	{
		BasePosition = Position;
		ShakeIntensity = intensity;
		if (duration != 0)
		{
			ShakeTimer.Start(duration);
		}
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
		Lib.Print("EXPLOSION POSITION : " + explosion.Position);
		Lib.Print("POSITION : " + Position + Size / 2);
		*/

		ShakeFinished();
	}

	public bool IsCollided(FloatWindow other)
	{
		Rect2I thisRect = new Rect2I(Position, Size);
		Rect2I otherRect = new Rect2I(other.Position, other.Size);

		return thisRect.Intersects(otherRect);
	}

	public void CheckCollision()
	{
		if (CollisionDisabled)
		{
			return;
		}
		foreach (FloatWindow window in GameManager.Windows)
		{
			if (window != this)
			{
				if (IsCollided(window))
				{
					if (!CollidedWindows.Contains(window))
					{
						CollidedWindows.Add(window);
						WindowCollided(window);
					}
				}
				else
				{
					if (CollidedWindows.Contains(window))
					{
						CollidedWindows.Remove(window);
						WindowNotCollided(window);
					}
				}
			}
		}
	}

	public virtual void WindowCollided(FloatWindow window)
	{
		//Lib.Print("Window Collided");
	}

	public virtual void WindowNotCollided(FloatWindow window)
	{
		//Lib.Print("Window Not Collided");
	}

}
