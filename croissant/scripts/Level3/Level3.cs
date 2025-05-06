using Godot;
using System;

public partial class Level3 : FloatWindow
{
	// Called when the node enters the scene tree for the first time.
	public Action<InputEventMouseButton> MouseEvent;
	public override void _Ready()
	{	
		GrabFocus();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
		{
			MouseEvent(mouseButtonEvent);
			Lib.Print("MouseEvent: " + mouseButtonEvent.Pressed);
		}
	}
}
