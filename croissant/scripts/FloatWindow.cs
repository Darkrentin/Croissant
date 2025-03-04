using Godot;
using System;
using System.Numerics;
using System.Text.RegularExpressions;

public partial class FloatWindow : Window
{

	public enum TransitionMode
	{
		Custom,
		Linear,
		Quadratic,
	}

	[Export] public bool Draggable = true;

	[Export] public TransitionMode transitionMode = TransitionMode.Custom;
	[Export] public Curve curve = new Curve();

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
		LoadCurve();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockDrag();
		TransitionWindow(delta);
		if(Input.IsMouseButtonPressed(MouseButton.Left))
		{
			StartTransition((Vector2I)GetMousePosition(),1);
		}

	}

	private void BlockDrag()
	{
		if(!Draggable)
		{
			if(HasFocus())
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

	private void LoadCurve()
	{
		switch(transitionMode)
		{
			case TransitionMode.Linear:
				curve = ResourceLoader.Load<Curve>("res://assests/curves/LinearCurve.tres");
				break;
			default:
				curve = new Curve();
				break;
		}
	}

	public void TransitionWindow(double delta)
	{
		if(IsTransitioning)
		{
			if(elapsedTime<TransitionTime)
			{
				elapsedTime += (float)delta;
				float t = Mathf.Clamp(elapsedTime / TransitionTime, 0f, 1f);
				float speedFactor = curve.Sample(t);
				SetWindowPosition((Vector2I)((Godot.Vector2)StartPosition).Lerp(TargetPosition, speedFactor));
			}
			else
			{
				IsTransitioning = false;
			}
		}
	}

	private bool SetWindowPosition(Vector2I newPosition)
	{
		int x = Position.X;
		int y = Position.Y;
		bool returnValue = true;

		if(newPosition.X<0)
		{
			x = 0;
			returnValue = false;
		}
		else if(newPosition.X > GameManager.ScreenSize.X-Size.X)
		{
			x = GameManager.ScreenSize.X-Size.X;
			returnValue = false;
		}
		else
		{
			x = newPosition.X;
		}

		if(newPosition.Y<0)
		{
			y = 0;
			returnValue = false;
		}
		else if(newPosition.Y > GameManager.ScreenSize.Y-Size.Y)
		{
			y = GameManager.ScreenSize.Y-Size.Y;
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
