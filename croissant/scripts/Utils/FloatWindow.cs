using Godot;
using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

public partial class FloatWindow : Window
{

	public enum TransitionMode
	{
		Linear,
		Exponential
	}

	//additionnal properties
	[Export] public bool Draggable = true;
	[Export] public bool Minimizable = true;

	[Export] public bool Shaking = false;
	[Export] public Timer ShakeTimer;

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

	[Export] private PackedScene ExplosionScene = ResourceLoader.Load<PackedScene>("uid://q2tedokw1ckw");


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetTree().AutoAcceptQuit = false; // Prevent the game from closing when the window is closed
		CloseRequested += OnClose; // Connect the close event to the OnClose function to catch the close event
		ShakeTimer = new Timer();
		AddChild(ShakeTimer);
		ShakeTimer.Timeout+=()=>{StopShake();};
		GameManager.Windows.Add(this);
		Title = "Window Added";
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockAction(); // Block action made by the player depending on the window properties
		TransitionWindow(delta); //Compute the transition of the window
		ProcessShake();
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
		if(!Minimizable)
		{
			if(Mode==ModeEnum.Minimized)
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
		if(IsTransitioning && !reset)
		{
			TargetPosition+=targetPosition-StartPosition;
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
	public void StartResize(Vector2I targetSize, float resizeTime, bool KeepCenter = true)
	{
		StartSize = Size;
		TargetSize = targetSize;
		ResizeTime = resizeTime;
		IsResizing = true;
		elapsedTimeResize = 0;

		if(KeepCenter)
		{
			Vector2I deltaSize = TargetSize - StartSize;
			Vector2I deltaPosition = new Vector2I(deltaSize.X / 2, deltaSize.Y / 2);
			Vector2I newPosition = Position - deltaPosition;
			transitionMode = resizeMode;
			StartTransition(newPosition, resizeTime);
		}
		
	}

	// Start a linear transition to a target position
	public void StartLinearTransition(Vector2I targetPosition, float transitionTime, bool reset = false)
	{
		transitionMode = TransitionMode.Linear;
		StartTransition(targetPosition, transitionTime,Smoothness, reset);
	}

	// Start an exponential transition to a target position
	public void StartExponentialTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		transitionMode = TransitionMode.Exponential;
		StartTransition(targetPosition, transitionTime, smoothness,reset);
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

	public void StartResizeDown(int nsize, float resizeTime)
	{
		StartResize(new Vector2I(Size.X, Size.Y + nsize), resizeTime, false);
	}

	public void StartResizeUp(int nsize, float resizeTime)
	{
		StartResize(new Vector2I(Size.X, Size.Y + nsize), resizeTime, false);
		StartTransition(new Vector2I(Position.X, Position.Y - nsize), resizeTime);
	}

	public void StartResizeRight(int nsize, float resizeTime)
	{
		StartResize(new Vector2I(Size.X + nsize, Size.Y), resizeTime, false);
	}

	public void StartResizeLeft(int nsize, float resizeTime)
	{
		StartResize(new Vector2I(Size.X + nsize, Size.Y), resizeTime, false);
		StartTransition(new Vector2I(Position.X - nsize, Position.Y), resizeTime);
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
						//GD.Print($"transition Time: {elapsedTimeTransition} Progress: {progress}");
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
						//GD.Print($"Transtition Time: {elapsedTimeTransition} Progress: {progress} ExpProgress: {expProgress}");

						// Compute the new position based on the exponential easing function
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, expProgress);
						break;

					default:
						GD.PushWarning("Invalid transition mode");
						newPosition = Position;
						break;
				}

				SetWindowPosition(newPosition);
			}
			else
			{
				// Ensure we end exactly at the target position
				SetWindowPosition(TargetPosition,true);
				IsTransitioning = false;
				TransitionFinished();

			}
		}
		if(IsResizing)
		{
			// Check if the resize is still in progress
			if(elapsedTimeResize<ResizeTime)
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
						//GD.Print($"resize Time: {elapsedTimeResize} Progress: {progress}");
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
						//GD.Print($"Resize Time: {elapsedTimeResize} Progress: {progress} ExpProgress: {expProgress}");
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
		if(SkipVerification)
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
		//GD.Print("Window Closed");
	}

	public virtual void TransitionFinished()
	{
		//GD.Print("Transition Finished");
	}

	public virtual void ShakeFinished()
	{
		//GD.Print("Shake Finished");
	}

	public virtual void ResizeFinished()
	{
		//GD.Print("Resize Finished");
	}

	public void ProcessShake()
	{
		if(Shaking)
		{
			if(IsTransitioning)
			{
				BasePosition = Position;
				return;
			}

			int offsetX = (int)Lib.rand.Next(-ShakeIntensity,ShakeIntensity+1);
			int offsetY = (int)Lib.rand.Next(-ShakeIntensity,ShakeIntensity+1);

			Vector2I ShakePosition = BasePosition + new Vector2I(offsetX,offsetY);

			SetWindowPosition(ShakePosition);
		}
	}

	public void StartShake(float duration, int intensity)
	{
		BasePosition = Position;
		ShakeIntensity = intensity;
		if(duration!=0)
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
		CpuParticles2D explosion = ExplosionScene.Instantiate<CpuParticles2D>();
		explosion.Position = Position + Size/2;
		GameManager.GameRoot.AddChild(explosion);
		ShakeFinished();
	}

	public bool IsCollided(FloatWindow window)
	{
		// Check if this window collides with another window
		Rect2I thisRect = new Rect2I(Position, Size);
		Rect2I otherRect = new Rect2I(window.Position, window.Size);

		// Collision occurs if rectangles overlap
		return thisRect.Intersects(otherRect);
	}

}
