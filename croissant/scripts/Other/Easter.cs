using Godot;
using System;

public partial class Easter : Node3D
{
	[Export] public Node3D EasterEgg;
	public override void _Ready()
	{
		EasterEgg.LookAt(new Vector3(0, 1, 0), Vector3.Up);
		EasterEgg.Rotation *= new Vector3(0, 1, 0);
		EasterEgg.Rotation += new Vector3(0, (float)Math.PI, 0);
	}
}
