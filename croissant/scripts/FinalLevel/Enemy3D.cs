using Godot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

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

	[Export] private AudioStreamPlayer3D Detection;
	[Export] private AudioStreamPlayer3D HitSound;
	[Export] private AudioStreamPlayer3D DeathSound;

	private Vector3 RotationAxis;
	private int currentShape;
	private List<Mesh> shapeSequence;
	private float rotationSpeed = 2.0f;
	public double MaxAgro = 5f;
	public bool CanHarmPlayer = true;
	public Vector3 RandomTargetPosition;

	private Tween FadeTween;
	private float OriginalVolume = 0f;
	private bool isFading = false;

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

		// Store original volume
		if (Detection != null)
		{
			OriginalVolume = Detection.VolumeDb;
		}
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
			navigationAgent3D.TargetPosition = GlobalPosition;
			FadeOutDetectionSound();
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
			if (!Detection.Playing)
			{
				StartDetectionSound();
			}
			if (CanHarmPlayer && player.GlobalPosition.DistanceTo(GlobalPosition) < 1.3f && player.Alive)
				FinalLevel.Instance.Death(GlobalPosition);
		}

	}
	private void UpdateShape()
	{
		Mesh.Mesh = shapeSequence[currentShape];
	}

	private void Destroy()
	{
		DeathSound.Play();
		
		// Use fade out instead of immediate stop
		DeathSound.Stop();
		
		HitSound.Stop();
		Collision.Position = new Vector3(0, 10, 0);
		AnimationPlayer.Play("Depop");
		FinalLevel.Instance.EnemyCount--;
		CanHarmPlayer = false;
	}

	public void OnBulletCollide()
	{
		if (currentShape == 3)
			Destroy();
		else
		{
			currentShape++;
			HitSound.Play();
			
			// Fade out detection sound temporarily during hit
			if (Detection.Playing)
			{
				var tempTween = CreateTween();
				tempTween.TweenProperty(Detection, "volume_db", OriginalVolume * 0.3f, 0.2f);
				tempTween.TweenProperty(Detection, "volume_db", OriginalVolume, 0.3f);
			}
			
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
		if( animationName == "Depop")
		{
			FinalLevel.Instance.maze.Enemies.Remove(this);
			QueueFree();
			return;
		}
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

	private void StartDetectionSound()
	{
		if (Detection == null) return;
		
		// Cancel any ongoing fade
		CancelFade();
		
		// Start with fade in effect
		Detection.VolumeDb = -80f; // Start muted
		Detection.Play();
		
		FadeTween = CreateTween();
		isFading = true;
		
		FadeTween.TweenProperty(Detection, "volume_db", OriginalVolume, 1.0f);
		FadeTween.TweenCallback(Callable.From(() => {
			isFading = false;
		}));
	}

	private void FadeOutDetectionSound()
	{
		if (Detection == null || !Detection.Playing || isFading) return;
		
		isFading = true;
		FadeTween = CreateTween();
		
		// Fade from current volume to silent
		FadeTween.TweenProperty(Detection, "volume_db", -80f, 1.5f);
		FadeTween.TweenCallback(Callable.From(() => {
			Detection.Stop();
			Detection.VolumeDb = OriginalVolume; // Reset for next time
			isFading = false;
		}));
	}

	private void CancelFade()
	{
		if (FadeTween != null)
		{
			FadeTween.Kill();
			FadeTween = null;
		}
		isFading = false;
	}

	public void StopDetectionSound()
	{
		if (Detection == null) return;
		
		if (Detection.Playing)
		{
			FadeOutDetectionSound();
		}
		else
		{
			CancelFade();
		}
	}
		
}
