using Godot;
using System;

public partial class Virus3D : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	public float MapWidth = 20;
	public float MaxRotation = 1f;
	public float RotationSmoothing = 5;
	[Export] public MeshInstance3D Base;

	[Export] public Node3D ShootSpawnA;
	[Export] public Node3D ShootSpawnB;
	public Timer GlitchTimer;
	
	// Boss attack timers
	public Timer FloppyDiskTimer;
	public Timer MovementTimer;
	public Timer FloorAttackTimer;
	
	// Attack state tracking
	public bool IsLavaActive = false;
	public bool IsLiftWallActive = false;

	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] Sprite2D HealthBar;
	public static Virus3D Instance;

	public int Hp = 10;

	public bool Phase = false;



	public override void _Ready()
	{
		Instance = this;
		AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		AnimationScreen.Travel("Angry");

		GlitchTimer = new Timer();
		GlitchTimer.Timeout += () =>
		{
			StopGlitch();
		};
		GlitchTimer.WaitTime = 0.5f;
		GlitchTimer.OneShot = true;
		AddChild(GlitchTimer);

		// Initialize boss attack timers
		FloppyDiskTimer = new Timer();
		FloppyDiskTimer.Timeout += () =>
		{
			if (Phase) // Phase 2
			{
				if (!IsLavaActive)
				{
					// Phase 2: Tire beaucoup de disquettes
					BossLevel.Instance.LaunchNFloppyDisk(Lib.rand.Next(5, 10), 0.3f);
				}
				else
				{
					// Phase 2: Tire moins mais avec lava active
					BossLevel.Instance.LaunchNFloppyDisk(Lib.rand.Next(4, 8), 0.3f);
				}
			}
			else // Phase 1
			{
				// Phase 1: Tire modérément
				BossLevel.Instance.LaunchNFloppyDisk(Lib.rand.Next(2, 8), 0.4f);
			}
		};
		FloppyDiskTimer.OneShot = false;
		AddChild(FloppyDiskTimer);

		MovementTimer = new Timer();
		MovementTimer.Timeout += () =>
		{
			if (Phase) // Phase 2
			{
				// Phase 2: Bouge moins mais plus agressivement
				float rotationAngle = Lib.rand.Next(-120, 120);
				float rotationSpeed = Lib.rand.Next(80, 120);
				Rotate(rotationAngle, rotationSpeed);
			}
			else // Phase 1
			{
				// Phase 1: Bouge beaucoup
				float rotationAngle = Lib.rand.Next(-180, 180);
				float rotationSpeed = Lib.rand.Next(60, 100);
				Rotate(rotationAngle, rotationSpeed);
			}
		};
		MovementTimer.OneShot = false;
		AddChild(MovementTimer);

		FloorAttackTimer = new Timer();
		FloorAttackTimer.Timeout += () =>
		{
			if (!IsLavaActive && !IsLiftWallActive)
			{
				BossLevel.Instance.FloorAttack();
			}
		};
		FloorAttackTimer.OneShot = false;
		AddChild(FloorAttackTimer);

		AnimationPlayer.AnimationFinished += OnAnimationFinished;
		HealthBar.Frame = 10 - Hp;
		
		// Start the boss attack patterns
		StartBossAttacks();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// Check phase transition based on HP
		bool shouldBePhase2 = (float)Hp / 10.0f <= 0.5f; // 50% HP or less
		
		if (shouldBePhase2 != Phase)
		{
			Phase = shouldBePhase2;
			if (Phase)
			{
				Lib.Print("Boss entering Phase 2!");
				BossAtkPhase2();
			}
			else
			{
				Lib.Print("Boss back to Phase 1!");
				BossAtkPhase1();
			}
		}
	}

	public void StartBossAttacks()
	{
		if (Phase)
		{
			BossAtkPhase2();
		}
		else
		{
			BossAtkPhase1();
		}
	}	public void BossAtkPhase1()
	{
		// Phase 1: Beaucoup de mouvement, tir modéré de disquettes + FloorAttack fréquent
		
		Lib.Print("Boss Phase 1 activated - FLOOR ATTACK FOCUS");
		
		// Tir de disquettes toutes les 4-7 secondes (moins fréquent)
		FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
		FloppyDiskTimer.Start();
		
		// Mouvement fréquent toutes les 2-4 secondes
		MovementTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
		MovementTimer.Start();
		
		// FloorAttack TRÈS fréquent en Phase 1 toutes les 3-5 secondes
		FloorAttackTimer.WaitTime = Lib.GetRandomNormal(3.0f, 5.0f);
		FloorAttackTimer.Start();
		Lib.Print("Phase 1: FloorAttack timer set to 3-5 seconds");
	}	public void BossAtkPhase2()
	{
		// Phase 2: Focus sur l'attaque Lava + tir intensif
		
		Lib.Print("Boss Phase 2 activated - LAVA ATTACK FOCUS");
		
		// Mode simple : Tir intensif + Attaque Lava fréquente
		IsLavaActive = true;
		
		// Tir plus fréquent de disquettes toutes les 1.5-3 secondes
		FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(1.5f, 3.0f);
		FloppyDiskTimer.Start();
		
		// Mouvement modéré toutes les 3-5 secondes
		MovementTimer.WaitTime = Lib.GetRandomNormal(3.0f, 5.0f);
		MovementTimer.Start();
		
		// Attaque Lava immédiate et répétée
		BossLevel.Instance.Lava();
		Lib.Print("Phase 2: Lava attack launched!");
		
		// Répéter l'attaque Lava toutes les 8-12 secondes
		GetTree().CreateTimer(Lib.GetRandomNormal(8.0f, 12.0f)).Timeout += () =>
		{
			if (Phase && IsLavaActive) // Si on est toujours en Phase 2
			{
				BossLevel.Instance.Lava();
				Lib.Print("Phase 2: Repeated Lava attack!");
				
				// Programmer la prochaine attaque
				GetTree().CreateTimer(Lib.GetRandomNormal(8.0f, 12.0f)).Timeout += () =>
				{
					if (Phase && IsLavaActive)
					{
						BossLevel.Instance.Lava();
						Lib.Print("Phase 2: Another Lava attack!");
					}
				};
			}
		};
		
		// FloorAttack moins fréquent en Phase 2 toutes les 8-15 secondes
		FloorAttackTimer.WaitTime = Lib.GetRandomNormal(8.0f, 15.0f);
		FloorAttackTimer.Start();
		Lib.Print("Phase 2: Lava priority mode activated");
	}

	public void ShakeNode(Node3D nodeToShake, float intensity = 0.3f, float duration = 0.5f)
	{
		Vector3 originalPosition = nodeToShake.Position;
		Tween shakeTween = CreateTween();
		int frequency = (int)(duration * 60);
		float timePerShake = duration / frequency;

		for (int i = 0; i < frequency; i++)
		{
			// Generate random offset
			Vector3 randomOffset = new Vector3(
				(float)Lib.rand.NextDouble() * intensity - intensity / 2,
				(float)Lib.rand.NextDouble() * intensity - intensity / 2,
				(float)Lib.rand.NextDouble() * intensity - intensity / 2
			);

			// Tween to random position
			shakeTween.TweenProperty(nodeToShake, "position", originalPosition + randomOffset, timePerShake / 2)
				.SetEase(Tween.EaseType.Out)
				.SetTrans(Tween.TransitionType.Sine);

			// Tween back to midpoint
			shakeTween.TweenProperty(nodeToShake, "position", originalPosition, timePerShake / 2)
				.SetEase(Tween.EaseType.In)
				.SetTrans(Tween.TransitionType.Sine);
		}

		// Ensure we return to original position
		shakeTween.TweenProperty(nodeToShake, "position", originalPosition, 0.01f);
	}	public void TakeDamage()
	{
		// Handle damage logic here
		// For example, reduce health or trigger an animation
		StartGlitch();
		
		// Toujours appeler LiftWalls quand le boss se fait tirer dessus
		IsLiftWallActive = true;
		BossLevel.Instance.LiftWalls();
		
		// Programmer la fin de LiftWalls
		GetTree().CreateTimer(3.0f).Timeout += () =>
		{
			IsLiftWallActive = false;
		};
		
		//Rotate(Lib.rand.Next(-180, 180), 60f, BossLevel.Instance.LaunchFloppyDisk);
		Lib.Print("Virus took damage!");
		if (Hp > 1)
		{
			Hp--;
			HealthBar.Frame = 10 - Hp;
		}
		else
		{
			Hp--;
			HealthBar.Frame = 10 - Hp;
			StartGlitch();
			GlitchTimer.WaitTime = 10f;
			
			// Stop all attack timers when boss dies
			FloppyDiskTimer.Stop();
			MovementTimer.Stop();
			FloorAttackTimer.Stop();
			
			GetTree().CreateTimer(1f).Timeout += () =>
			{
				FinalLevel.Instance.AnimationPlayer.Play("BossDeath");
				FinalLevel.Instance.Player3D.ProcessMode = ProcessModeEnum.Disabled;
				SpeedRunTimer.Instance.StopTimer();
				GlitchTimer.Stop();
			};
		}
	}

	public void HealVirus()
	{
		if (Hp < 10)
		{
			Hp++;
			HealthBar.Frame = 10 - Hp;
		}
	}

	public void StartGlitch()
	{
		ShaderMaterial shaderMaterial = (ShaderMaterial)Base.MaterialOverride;
		shaderMaterial.SetShaderParameter("shake_rate", 1);
		GlitchTimer.Start();
	}

	public void StopGlitch()
	{
		ShaderMaterial shaderMaterial = (ShaderMaterial)Base.MaterialOverride;
		shaderMaterial.SetShaderParameter("shake_rate", 0);
		GlitchTimer.Stop();
	}

	public void OnAnimationFinished(StringName anim)
	{
	}
	public void Rotate(float targetAngleDegrees, float rotationSpeed, Action onCompleteCallback = null)
	{
		float targetAngleRadians = Mathf.DegToRad(targetAngleDegrees);
		Vector3 targetRotation = new Vector3(Rotation.X, targetAngleRadians, Rotation.Z);

		float currentAngleRadians = Rotation.Y;
		float angleDifference = targetAngleRadians - currentAngleRadians;

		while (angleDifference > Mathf.Pi)
		{
			angleDifference -= 2 * Mathf.Pi;
		}
		while (angleDifference < -Mathf.Pi)
		{
			angleDifference += 2 * Mathf.Pi;
		}

		float rotationSpeedRadians = Mathf.DegToRad(rotationSpeed);
		float duration = Mathf.Abs(angleDifference) / rotationSpeedRadians;

		duration = Mathf.Max(duration, 0.1f);

		Tween rotationTween = CreateTween();
		rotationTween.SetTrans(Tween.TransitionType.Linear);
		rotationTween.SetEase(Tween.EaseType.InOut);

		rotationTween.TweenProperty(this, "rotation", targetRotation, duration);

		if (onCompleteCallback != null)
		{
			rotationTween.Finished += () => onCompleteCallback.Invoke();
		}
	}

	public float GetRotatePlus(float n)
	{
		return Rotation.Y + n;
	}

}
