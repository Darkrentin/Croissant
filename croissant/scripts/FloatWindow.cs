using Godot;
using System;
using System.Numerics;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

public partial class FloatWindow : Window
{

	public enum TransitionMode
	{
		Linear,
		Exponential
	}

	[Export] public bool Draggable = true;

	[Export] public TransitionMode transitionMode = TransitionMode.Linear;
	[Export] public float Smoothness = 5.0f;

	private Vector2I TargetPosition;
	private Vector2I StartPosition;
	private float TransitionTime = 0.5f;
	private float elapsedTime = 0;
	public bool IsTransitioning = false;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitialPosition = WindowInitialPosition.CenterPrimaryScreen;
		//Position = new Vector2I(500, 500);
		Size = new Vector2I(400, 400);
		TargetPosition = Position;
		// Removed LoadCurve() call
		DelayMethod();
		//StartTransition(new Vector2I(500, 500), 4);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockDrag();
		TransitionWindow(delta);
		//if (Input.IsMouseButtonPressed(MouseButton.Left) && !IsTransitioning)
		//{
		//	StartTransition((Vector2I)GetMousePosition(), 4);
		//}
	}
	private async Task DelayMethod()
	{
		StartTransition(new Vector2I(200, 500), 5);
		await Task.Delay(TimeSpan.FromMilliseconds(5500));
		StartTransition(new Vector2I(1000, 500), 0.5f);
		await Task.Delay(TimeSpan.FromMilliseconds(3000));
		DelayMethod();
	}

	private void BlockDrag()
	{
		if (!Draggable)
		{
			if (HasFocus())
			{
				GameManager.FixWindow.GrabFocus();
			}
		}
	}

	public void StartTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f)
	{
		StartPosition = Position;
		TargetPosition = targetPosition;
		TransitionTime = transitionTime;
		IsTransitioning = true;
		elapsedTime = 0;
		Smoothness = smoothness;
	}
	public void StartLinearTransition(Vector2I targetPosition, float transitionTime)
	{
		transitionMode = TransitionMode.Linear;
		StartTransition(targetPosition, transitionTime);
	}
	public void StartExponentialTransition(Vector2I targetPosition, float transitionTime, float smoothness = 5.0f)
	{
		transitionMode = TransitionMode.Exponential;
		StartTransition(targetPosition, transitionTime, smoothness);
	}

	public void TransitionWindow(double delta)
	{
		if (IsTransitioning)
		{
			if (elapsedTime < TransitionTime)
			{
				elapsedTime += (float)delta;
				Vector2I newPosition;
				
				// Normalized progress from 0.0 to 1.0
				float progress = Mathf.Clamp(elapsedTime / TransitionTime, 0f, 1f);

				switch (transitionMode)
				{
					case TransitionMode.Linear:
						// Linear interpolation - moves at constant speed
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, progress);
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
						GD.Print($"Time: {elapsedTime} Progress: {progress} ExpProgress: {expProgress}");
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, expProgress);
						break;

					default:
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
	}

	private bool SetWindowPosition(Vector2I newPosition)
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
}
