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
	[Export] private RayCast3D rayCast;
	[Export] private PackedScene EnemyHitScene;
	[Export] private PackedScene EnemyExplosionScene;
	[Export] public bool UpdateShapeButton { get => false; set => UpdateShape(); }
	[Export] public bool ExplosionButton { get => false; set { if (value) AddExplosion(); } }
	[Export] private float MovementSpeed = 1.0f;
	private Vector3 RotationAxis;
	private int currentShape;
	private List<Mesh> shapeSequence;
	private float rotationSpeed = 2.0f;
	public double MaxAgro = 5f;
	public bool CanHarmPlayer = true;
	public Vector3 RandomTargetPosition;

	public double Agro = 0f;

	public override void _Ready()
	{
		RotationAxis = new Vector3(Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1), Lib.GetRandomNormal(-1, 1));
		shapeSequence = new List<Mesh> { IcosahedronMesh, DodecahedronMesh, CubeMesh, TetrahedronMesh };
		currentShape = Lib.rand.Next(0, 4);
		UpdateShape();
		if (currentShape == 3)
		{
			Scale = new Vector3(0.75f, 0.75f, 0.75f);
			Position += new Vector3(0, 0.4f, 0);
		}
		AnimationPlayer.AnimationFinished += OnAnimationFinished;
		navigationAgent3D.DebugEnabled = FinalLevel.Instance.Debug;
		RandomTargetPosition = GlobalPosition;
	}

	public override void _PhysicsProcess(double delta)
	{
		Mesh.Rotation += RotationAxis * (float)delta * rotationSpeed;

		if (!CanHarmPlayer)
		{
			navigationAgent3D.Velocity = Vector3.Zero;
			return;
		}
		if (Agro > 0f)
		{
			Agro -= delta;
			if (Agro < 0f) Agro = 0f;
			navigationAgent3D.TargetPosition = FinalLevel.Instance.Player3D.GlobalPosition;
			rayCast.Visible = true && FinalLevel.Instance.Debug;
		}
		else
		{
			if (GlobalPosition.DistanceTo(FinalLevel.Instance.Player3D.GlobalPosition) < 10f)
				rayCast.Visible = true && FinalLevel.Instance.Debug;
			else
				rayCast.Visible = false;
			//navigationAgent3D.TargetPosition = GlobalPosition;

			if (GlobalPosition.DistanceTo(RandomTargetPosition) < 0.5f)
			{
				//int RandomPathIndex = Lib.rand.Next(0,FinalLevel.Instance.maze.Paths.Count);
				//RandomTargetPosition = FinalLevel.Instance.maze.Paths[RandomPathIndex].GlobalPosition + new Vector3(1,0,1);
				//Lib.Print("Random Target Position: " + RandomTargetPosition);
			}

			navigationAgent3D.TargetPosition = RandomTargetPosition;


		}

		//navigationAgent3D.TargetPosition = FinalLevel.Instance.Player3D.GlobalPosition;// ((FinalLevel.Instance.Player3D.GlobalPosition/2)*2) + new Vector3(1,0,1);
		Vector3 nextPathPosition = navigationAgent3D.GetNextPathPosition();
		Vector3 IntendedVelocity = GlobalTransform.Origin.DirectionTo(nextPathPosition) * MovementSpeed;
		navigationAgent3D.Velocity = IntendedVelocity;

		Vector3 directionToPlayer = FinalLevel.Instance.Player3D.GlobalPosition - rayCast.GlobalPosition + new Vector3(0, 0.75f, 0);
		rayCast.TargetPosition = rayCast.ToLocal(rayCast.GlobalPosition + directionToPlayer);

		rayCast.ForceRaycastUpdate();


		if (rayCast.GetCollider() is Player3D player && rayCast.IsEnabled())
		{
			Agro = MaxAgro;
			if (CanHarmPlayer && player.GlobalPosition.DistanceTo(GlobalPosition) < 1.3f && player.Alive)
				FinalLevel.Instance.Death(GlobalPosition);
		}

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

			AnimationPlayer.Play("ShapeChange");
			CanHarmPlayer = false;
			GpuParticles3D enemyHit = EnemyHitScene.Instantiate<GpuParticles3D>();
			AddChild(enemyHit);
			enemyHit.GlobalPosition = GlobalPosition + new Vector3(0, 0.6f, 0);

			if (currentShape == 3)
			{
				Scale = new Vector3(0.75f, 0.75f, 0.75f);
				Position += new Vector3(0, 0.4f, 0);
			}
		}
	}

	public void OnAnimationFinished(StringName animationName)
	{
		if (animationName == "ShapeChange")
			CanHarmPlayer = true;
	}

	private void Destroy()
	{
		Collision.Position = new Vector3(0, 10, 0);
		AnimationPlayer.Play("Depop");
		FinalLevel.Instance.EnemyCount--;
		CanHarmPlayer = false;
	}

	public void _on_navigation_agent_3d_velocity_computed(Vector3 velocity)
	{
		velocity = velocity * new Vector3(1, 0, 1);
		Velocity = velocity;
		MoveAndSlide();
	}

	public void AddExplosion()
	{
		GpuParticles3D explosion = EnemyExplosionScene.Instantiate<GpuParticles3D>();
		AddChild(explosion);
		explosion.GlobalPosition = GlobalPosition + new Vector3(0, 0.6f, 0);
	}
}
