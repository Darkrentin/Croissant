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
	[Export] public float transitionSpeed = 10.0f; // Speed factor for exponential transition

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
		StartTransition(new Vector2I(500, 500), 2);
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
		StartTransition(new Vector2I(1500, 500), 2);
		await Task.Delay(TimeSpan.FromMilliseconds(1000));
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

	public void StartTransition(Vector2I targetPosition, float transitionTime)
	{
		StartPosition = Position;
		TargetPosition = targetPosition;
		TransitionTime = transitionTime;
		IsTransitioning = true;
		elapsedTime = 0;
	}

	public void TransitionWindow(double delta)
	{
		if (IsTransitioning)
		{
			if (elapsedTime < TransitionTime)
			{
				elapsedTime += (float)delta;
				Vector2I newPosition;

				switch (transitionMode)
				{
					case TransitionMode.Linear:
						// Linear interpolation - moves at constant speed
						float t = Mathf.Clamp(elapsedTime / TransitionTime, 0f, 1f);
						newPosition = (Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, t);
						break;

					case TransitionMode.Exponential:
						// Exponential smoothing
						// position += (target - position) * (1 - exp(-dt * speed))
						float dt = (float)delta;
						Godot.Vector2 currentPos = Position;
						Godot.Vector2 target = TargetPosition;
						Godot.Vector2 difference = target - currentPos;

						// Apply exponential smoothing formula
						Godot.Vector2 movement = difference * (1 - Mathf.Exp(-dt * transitionSpeed));

						// Scale the movement to ensure we complete in TransitionTime
						// As time approaches TransitionTime, we force completion
						float completionFactor = Mathf.Clamp(elapsedTime / TransitionTime, 0f, 0.99f);
						float boost = 1.0f / (1.0f - completionFactor);
						movement *= boost;

						newPosition = (Vector2I)(currentPos + movement);
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
