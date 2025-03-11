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

	[Export] public bool Draggable = true;
	[Export] public bool Minimizable = true;

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


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		CloseRequested += OnClose;
		/*
		InitialPosition = WindowInitialPosition.CenterPrimaryScreen;
		//Position = new Vector2I(500, 500);
		Size = new Vector2I(400, 400);
		TargetPosition = Position;
		// Removed LoadCurve() call
		DelayMethod();
		//StartTransition(new Vector2I(500, 500), 4);
		*/
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockAction();
		TransitionWindow(delta);
		//if (Input.IsMouseButtonPressed(MouseButton.Left) && !IsTransitioning)
		//{
		//	StartTransition((Vector2I)GetMousePosition(), 4);
		//}
	}

	private void BlockAction()
	{
		if (!Draggable)
		{
			if (HasFocus())
			{
				GameManager.FixWindow.GrabFocus();
			}
		}
		if(!Minimizable)
		{
			if(Mode==ModeEnum.Minimized)
			{
				Mode = ModeEnum.Windowed;
			}
		}
	}

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

	public void StartResize(Vector2I targetSize, float resizeTime)
	{
		StartSize = Size;
		TargetSize = targetSize;
		ResizeTime = resizeTime;
		IsResizing = true;
		elapsedTimeResize = 0;

		Vector2I deltaSize = TargetSize - StartSize;
		Vector2I deltaPosition = new Vector2I(deltaSize.X / 2, deltaSize.Y / 2);
		Vector2I newPosition = Position - deltaPosition;
		transitionMode = resizeMode;
		StartTransition(newPosition, resizeTime);
		
	}
	public void StartLinearTransition(Vector2I targetPosition, float transitionTime, bool reset = false)
	{
		transitionMode = TransitionMode.Linear;
		StartTransition(targetPosition, transitionTime,Smoothness, reset);
	}
	public void StartExponentialTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f, bool reset = false)
	{
		transitionMode = TransitionMode.Exponential;
		StartTransition(targetPosition, transitionTime, smoothness,reset);
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

	private void TransitionWindow(double delta)
	{
		if (IsTransitioning)
		{
			if (elapsedTimeTransition < TransitionTime)
			{
				elapsedTimeTransition += (float)delta;
				Vector2I newPosition;

				// Normalized progress from 0.0 to 1.0
				float progress = Mathf.Clamp(elapsedTimeTransition / TransitionTime, 0f, 1f);

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
				SetWindowPosition(TargetPosition);
				IsTransitioning = false;
			}
		}
		if(IsResizing)
		{
			if(elapsedTimeResize<ResizeTime)
			{
				elapsedTimeResize += (float)delta;
				Vector2I newSize;
				float progress = Mathf.Clamp(elapsedTimeResize / ResizeTime, 0f, 1f);
				switch (resizeMode)
				{
					case TransitionMode.Linear:
						newSize = (Vector2I)((Godot.Vector2)StartSize).Lerp(TargetSize, progress);
						//GD.Print($"resize Time: {elapsedTimeResize} Progress: {progress}");
						break;
					case TransitionMode.Exponential:
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
			}
		}
	}

	public bool SetWindowPosition(Vector2I newPosition)
	{
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

	protected virtual void OnClose()
	{
		GD.Print("Window Closed");
	}
}
