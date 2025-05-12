using Godot;
using System;

public partial class Easter : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Node3D EasterEgg;
	public override void _Ready()
	{
		EasterEgg.LookAt(new Vector3(0, 1, 0), Vector3.Up);
		EasterEgg.Rotation *= new Vector3(0, 1, 0);
		EasterEgg.Rotation += new Vector3(0, (float)Math.PI, 0);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
