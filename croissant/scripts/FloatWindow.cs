using Godot;
using System;

public partial class FloatWindow : Window
{

	public enum TransitionMode
	{
		Custom,
		Linear,
		Quadratic,
		Cubic,
	}

	[Export] public bool Draggable = true;
	public Vector2I TargetPosition;

	[Export] public TransitionMode transitionMode = TransitionMode.Custom;
	[Export] public Curve curve;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		InitialPosition = WindowInitialPosition.CenterPrimaryScreen;
		//Position = new Vector2I(500, 500);
		Size = new Vector2I(400, 400);
		TargetPosition = Position;

		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		BlockDrag();
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
