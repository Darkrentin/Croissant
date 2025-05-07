using Godot;
using System.Collections.Generic;

public partial class Enemy3D : Area3D
{
	[Export] private MeshInstance3D Mesh;
	[Export] private Mesh IcosahedronMesh;
	[Export] private Mesh DodecahedronMesh;
	[Export] private Mesh CubeMesh;
	[Export] private Mesh TetrahedronMesh;
	[Export] private AnimationPlayer AnimationPlayer;
	[Export] private CollisionShape3D Collision;
	private Vector3 RotationAxis;
	private int currentShapeIndex;
	private List<Mesh> shapeSequence;
	public bool Alive = true;
	private float rotationSpeed = 2.0f;
	private int totalShapes = 4;

	public override void _Ready()
	{
		RotationAxis = new Vector3(Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1));
		shapeSequence = new List<Mesh> { IcosahedronMesh, DodecahedronMesh, CubeMesh, TetrahedronMesh };
		currentShapeIndex = Lib.rand.Next(0, totalShapes);
		UpdateShape();
	}

	public override void _Process(double delta)
	{
		Rotation += RotationAxis * (float)delta * rotationSpeed;
		Mesh.Rotation = Rotation;
	}

	private void UpdateShape()
	{
		Mesh.Mesh = shapeSequence[currentShapeIndex];
	}

	public void OnBulletCollide()
	{
		if (!Alive) return;
		currentShapeIndex++;
		if (currentShapeIndex >= totalShapes)
			Destroy();
		else
			UpdateShape();
	}

	private void Destroy()
	{
		Alive = false;
		Collision.Disabled = true;
		AnimationPlayer.Play("Depop");
		Mesh.Visible = false;
		Timer destroyTimer = new Timer();
		destroyTimer.WaitTime = 0.5f;
		destroyTimer.OneShot = true;
		AddChild(destroyTimer);
		destroyTimer.Timeout += () => QueueFree();
		destroyTimer.Start();
	}
}
