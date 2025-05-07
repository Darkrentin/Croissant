using Godot;
using System;

public partial class Enemy3D : Node3D
{
	[Export] private MeshInstance3D Mesh;
	private Vector3 RotationAxis; // Axe de rotation (Y)

	public override void _Ready()
	{
		RotationAxis = new Vector3(Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1));
	}


	public override void _Process(double delta)
	{
		Rotation += RotationAxis * (float)delta * 2f;
		Mesh.Rotation = Rotation;
	}
}
