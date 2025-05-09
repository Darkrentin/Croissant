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
	private int currentShape;
	private List<Mesh> shapeSequence;
	private float rotationSpeed = 2.0f;
	[Export] private float movementSpeed = 1.0f;
	[Export] private RayCast3D rayCast;
	public double MaxAgro = 5f;

	public double Agro = 0f;

	public override void _Ready()
	{
		RotationAxis = new Vector3(Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1));
		shapeSequence = new List<Mesh> { IcosahedronMesh, DodecahedronMesh, CubeMesh, TetrahedronMesh };
		currentShape = Lib.rand.Next(0, 4);
		UpdateShape();
	}

	public override void _Process(double delta)
	{
		Mesh.Rotation += RotationAxis * (float)delta * rotationSpeed;
	}

	public override void _PhysicsProcess(double delta)
	{
		navigationAgent3D.TargetPosition = FinalLevel.Instance.Player3D.GlobalPosition;// ((FinalLevel.Instance.Player3D.GlobalPosition/2)*2) + new Vector3(1,0,1);
		Vector3 nextPathPosition = navigationAgent3D.GetNextPathPosition();
		Vector3 IntendedVelocity = GlobalTransform.Origin.DirectionTo(nextPathPosition) * movementSpeed;
		navigationAgent3D.Velocity = IntendedVelocity;

		Vector3 directionToPlayer = FinalLevel.Instance.Player3D.GlobalPosition - rayCast.GlobalPosition + new Vector3(0, 0.75f, 0);
		rayCast.TargetPosition = rayCast.ToLocal(rayCast.GlobalPosition + directionToPlayer);

		if (rayCast.GetCollider() is Player3D player && rayCast.IsEnabled())
		{
			Agro = MaxAgro;
			if (player.GlobalPosition.DistanceTo(GlobalPosition) < 1.3f)
				FinalLevel.Instance.Death(GlobalPosition);
		}
		else if (Agro > 0f)
		{
			Agro -= delta;
			if (Agro < 0f) Agro = 0f;
		}
		else
			navigationAgent3D.Velocity = Vector3.Zero;

	}
	private void UpdateShape()
	{
		Mesh.Mesh = shapeSequence[currentShape];
	}

	public void OnBulletCollide()
	{
		if (currentShape == 3)
			Destroy();
		else
		{
			currentShape++;
			if (currentShape == 3)
			{
				Scale = new Vector3(0.8f, 0.8f, 0.8f);
				Position += new Vector3(0, 0.25f, 0);
			}

			Timer ShapeChangeTimer = new Timer();
			ShapeChangeTimer.WaitTime = 0.25f;
			ShapeChangeTimer.OneShot = true;
			AddChild(ShapeChangeTimer);
			ShapeChangeTimer.Timeout += () => UpdateShape();
			ShapeChangeTimer.Start();
			AnimationPlayer.Play("ShapeChange");
		}
	}

	private void Destroy()
	{
		Collision.Position = new Vector3(0, 10, 0);
		AnimationPlayer.Play("Depop");
		Timer destroyTimer = new Timer();
		destroyTimer.WaitTime = 0.5f;
		destroyTimer.OneShot = true;
		AddChild(destroyTimer);
		destroyTimer.Timeout += () => QueueFree();
		destroyTimer.Start();
		FinalLevel.Instance.EnemyCount--;
	}

	public void _on_navigation_agent_3d_velocity_computed(Vector3 velocity)
	{
		velocity = velocity * new Vector3(1, 0, 1);
		Velocity = velocity;
		MoveAndSlide();
	}
}