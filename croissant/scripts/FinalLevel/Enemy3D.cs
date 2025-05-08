using Godot;
using System.Collections.Generic;

public partial class Enemy3D : CharacterBody3D
{
	[Export] private MeshInstance3D Mesh;
	[Export] private Mesh IcosahedronMesh;
	[Export] private Mesh DodecahedronMesh;
	[Export] private Mesh CubeMesh;
	[Export] private Mesh TetrahedronMesh;
	[Export] private AnimationPlayer AnimationPlayer;
	[Export] private CollisionShape3D Collision;
	[Export] private NavigationAgent3D navigationAgent3D;
	private Vector3 RotationAxis;
	private int currentShapeIndex;
	private List<Mesh> shapeSequence;
	public bool Alive = true;
	private float rotationSpeed = 1.0f;
	[Export] private float movementSpeed = 5.0f;

	public override void _Ready()
	{
		RotationAxis = new Vector3(Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1));
		shapeSequence = new List<Mesh> { IcosahedronMesh, DodecahedronMesh, CubeMesh, TetrahedronMesh };
		currentShapeIndex = Lib.rand.Next(0, 4);
		UpdateShape();
	}

	public override void _Process(double delta)
	{
		Rotation += RotationAxis * (float)delta * rotationSpeed;
		Mesh.Rotation = Rotation;
		
	}

    public override void _PhysicsProcess(double delta)
    {
        navigationAgent3D.TargetPosition = FinalLevel.Instance.Player3D.GlobalPosition;
		Vector3 nextPathPosition = navigationAgent3D.GetNextPathPosition();
		Velocity = GlobalTransform.Origin.DirectionTo(nextPathPosition) * movementSpeed;
		MoveAndSlide();

    }
	private void UpdateShape()
	{
		Mesh.Mesh = shapeSequence[currentShapeIndex];
	}

	public void OnBulletCollide()
	{
		if (!Alive) return;
		currentShapeIndex++;
		if (currentShapeIndex >= 4)
			Destroy();
		else
			UpdateShape();
	}

	private void Destroy()
	{
		Alive = false;
		Collision.Position = new Vector3(0, 10, 0);
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
