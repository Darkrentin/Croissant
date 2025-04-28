using Godot;
using System;
using System.Runtime.InteropServices;

public partial class BlockWindow : FloatWindow
{
	// Called when the node enters the scene tree for the first time.
	[Export] public StaticBody2D sp;
	public Vector2I Spsize;
	public override void _Ready()
	{
		Spsize = new Vector2I(16, 16);
		base._Ready();
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		Lib.Print("Position: " + Position + " Size: " + Size);
		if(sp.Position != Position + Size/2)
			sp.Position = Position + Size/2;

		Vector2 newSize = Size/Spsize;
		if (sp.Scale != newSize)
			sp.Scale = newSize;
	}

    public override void OnClose()
    {
		QueueFree();
		GetTree().Quit();
    }

}
