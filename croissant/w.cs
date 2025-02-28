using Godot;
using System;

public partial class w : Window
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		TransparentBg = true;
		Transparent = true;
		AlwaysOnTop = true;
		Borderless = true;
		Unfocusable = true;
		Unresizable = true;
		

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
