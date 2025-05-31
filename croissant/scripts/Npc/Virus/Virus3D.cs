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

	[Export] public AudioStreamPlayer BossExplosion1Sound;
	[Export] public AudioStreamPlayer BossExplosion2Sound;
	[Export] public AudioStreamPlayer BossHitSound;
	public Timer GlitchTimer;
	
	// Boss attack timers
	public Timer FloppyDiskTimer;
	public Timer MovementTimer;
	
	// Attack state tracking
	public bool IsLavaActive = false;
	public bool IsLiftWallActive = false;

	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] Sprite2D HealthBar;
	public static Virus3D Instance;

	public int MaxHp = 30; // 3 fois plus d'HP
	public int Hp = 30;

	public bool Phase = false;

// Ajout pour gérer LiftWalls aléatoirement
public Timer LiftWallTimer;
public bool IsLiftWallOnCooldown = false;

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
    FloppyDiskTimer.Timeout += LaunchFloppyDiskAttack;
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


    // Nouveau timer pour LiftWalls aléatoire
    LiftWallTimer = new Timer();
    LiftWallTimer.Timeout += TryLaunchLiftWall;
    LiftWallTimer.OneShot = false;
    AddChild(LiftWallTimer);

    AnimationPlayer.AnimationFinished += OnAnimationFinished;
    UpdateHealthBar();
    
    // Start with Phase 1
    BossAtkPhase1();
    
    // Démarrer le timer pour LiftWalls aléatoire
    LiftWallTimer.WaitTime = Lib.GetRandomNormal(8.0f, 15.0f);
    LiftWallTimer.Start();
}

// Called every frame. 'delta' is the elapsed time since the previous frame.
public override void _Process(double delta)
{
    // Check phase transition based on HP
    bool shouldBePhase2 = (float)Hp / (float)MaxHp <= 0.5f; // 50% HP or less
    
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
    
    // Debug: Check if timers are running (remove this after debugging)
    if (Input.IsActionJustPressed("ui_accept")) // Press Enter to debug
    {
        DebugTimerStatus();
    }
}

private void UpdateHealthBar()
{
    // Convertir les HP sur 30 en affichage sur 10
    int displayHp = Mathf.RoundToInt((float)Hp / (float)MaxHp * 10f);
    displayHp = Mathf.Clamp(displayHp, 0, 10);
    HealthBar.Frame = 10 - displayHp;
}

private void LaunchFloppyDiskAttack()
{
    // Calculer le nombre de disquettes selon les HP restants
    float hpPercentage = (float)Hp / (float)MaxHp;
    int baseFloppyCount;
    
    if (Phase) // Phase 2
    {
        if (IsLavaActive)
        {
            // Pendant la lave, tire forcément mais peu
            baseFloppyCount = 2;
        }
        else
        {
            // Phase 2 sans lave: commence à 3 et augmente
            baseFloppyCount = 3 + Mathf.RoundToInt((1f - hpPercentage) * 5f); // 3-8 disquettes
        }
    }
    else // Phase 1
    {
        // Phase 1: commence à 1 et augmente progressivement
        baseFloppyCount = 1 + Mathf.RoundToInt((1f - hpPercentage) * 4f); // 1-5 disquettes
    }
    
    // Ajouter un peu d'aléatoire
    int floppyCount = Mathf.Max(1, baseFloppyCount + Lib.rand.Next(-1, 2));
    
    float launchSpeed = Phase ? 0.3f : 0.4f;
    
    BossLevel.Instance.LaunchNFloppyDisk(floppyCount, launchSpeed);
    Lib.Print($"Launched {floppyCount} floppies (HP: {Hp}/{MaxHp}, Phase: {(Phase ? 2 : 1)})");
}

private void TryLaunchLiftWall()
{
    // Ne pas lancer LiftWall si la lave est active ou si on est en cooldown
    if (!IsLavaActive && !IsLiftWallOnCooldown)
    {
        IsLiftWallActive = true;
        IsLiftWallOnCooldown = true;
        BossLevel.Instance.LiftWalls();
        Lib.Print("Random LiftWall attack launched!");
        
        // Programmer la fin de LiftWalls après 3 secondes
        GetTree().CreateTimer(3.0f).Timeout += () =>
        {
            IsLiftWallActive = false;
        };
        
        // Cooldown de 5 secondes avant de pouvoir relancer
        GetTree().CreateTimer(5.0f).Timeout += () =>
        {
            IsLiftWallOnCooldown = false;
        };
    }
    
    // Programmer le prochain essai
    LiftWallTimer.WaitTime = Lib.GetRandomNormal(8.0f, 15.0f);
    LiftWallTimer.Start();
}

	public void BossAtkPhase1()
	{
		// Phase 1: Beaucoup de mouvement, tir modéré de disquettes
		
		Lib.Print("Boss Phase 1 activated - FLOOR ATTACK FOCUS");
		
		// Stop all timers first to avoid conflicts
		FloppyDiskTimer.Stop();
		MovementTimer.Stop();
		
		// Tir de disquettes toutes les 3-5 secondes
		FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(3.0f, 5.0f);
		FloppyDiskTimer.Start();
		Lib.Print($"FloppyDisk timer started with {FloppyDiskTimer.WaitTime}s");
		
		// Mouvement fréquent toutes les 2-4 secondes
		MovementTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
		MovementTimer.Start();
		Lib.Print($"Movement timer started with {MovementTimer.WaitTime}s");
			}

	public void BossAtkPhase2()
	{
		// Phase 2: Focus sur l'attaque Lava + tir intensif

		Lib.Print("Boss Phase 2 activated - LAVA ATTACK FOCUS");

		// Stop all timers first
		FloppyDiskTimer.Stop();
		MovementTimer.Stop();

		// Mode simple : Tir intensif + Attaque Lava fréquente
		IsLavaActive = true;

		// Tir plus fréquent pendant la lave
		FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(1.0f, 2.0f);
		FloppyDiskTimer.Start();

		// Mouvement modéré toutes les 3-5 secondes
		MovementTimer.WaitTime = Lib.GetRandomNormal(3.0f, 5.0f);
		MovementTimer.Start();

		// Attaque Lava immédiate et répétée
		BossLevel.Instance.Lava();
		Lib.Print("Phase 2: Lava attack launched!");

		// Programmer la fin de la lave après 6 secondes
		GetTree().CreateTimer(6.0f).Timeout += () =>
		{
			IsLavaActive = false;
			// Retour à un tir normal après la lave
			FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
		};

		
		// Répéter l'attaque Lava toutes les 8-12 secondes
		GetTree().CreateTimer(Lib.GetRandomNormal(8.0f, 12.0f)).Timeout += () =>
		{
			if (Phase) // Si on est toujours en Phase 2
			{
				IsLavaActive = true;
				BossLevel.Instance.Lava();
				Lib.Print("Phase 2: Repeated Lava attack!");

				// Programmer la fin de cette lave
				GetTree().CreateTimer(6.0f).Timeout += () =>
				{
					IsLavaActive = false;
					FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
				};

				// Programmer la prochaine attaque
				GetTree().CreateTimer(Lib.GetRandomNormal(8.0f, 12.0f)).Timeout += () =>
				{
					if (Phase)
					{
						IsLavaActive = true;
						BossLevel.Instance.Lava();
						Lib.Print("Phase 2: Another Lava attack!");

						GetTree().CreateTimer(6.0f).Timeout += () =>
						{
							IsLavaActive = false;
							FloppyDiskTimer.WaitTime = Lib.GetRandomNormal(2.0f, 4.0f);
						};
					}
				};
			}
		};

		Lib.Print("Phase 2: Lava priority mode activated");
	}

	// Add a debug method to check timer status
	public void DebugTimerStatus()
	{
		Lib.Print("=== Timer Status Debug ===");
		Lib.Print($"FloppyDiskTimer - Stopped: {FloppyDiskTimer.IsStopped()}, WaitTime: {FloppyDiskTimer.WaitTime}, TimeLeft: {FloppyDiskTimer.TimeLeft}");
		Lib.Print($"MovementTimer - Stopped: {MovementTimer.IsStopped()}, WaitTime: {MovementTimer.WaitTime}, TimeLeft: {MovementTimer.TimeLeft}");
		Lib.Print($"Current Phase: {(Phase ? 2 : 1)}, HP: {Hp}/{MaxHp}");
		Lib.Print($"BossLevel.Instance exists: {BossLevel.Instance != null}");
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
		StartGlitch();
		BossHitSound.Play();
		
		// Ne plus lancer LiftWalls automatiquement quand on prend des dégâts
		// (c'est maintenant géré par le timer aléatoire)

		Lib.Print("Virus took damage!");
		if (Hp > 1)
		{
			Hp--;
			UpdateHealthBar();
		}
		else
		{
			Hp--;
			UpdateHealthBar();
			EndActions();
			
		}
	}

	public void EndActions()
	{
		StartGlitch();
		GlitchTimer.WaitTime = 10f;
			
		// Stop all attack timers when boss dies
		FloppyDiskTimer.Stop();
		MovementTimer.Stop();
		LiftWallTimer.Stop();
			
		GetTree().CreateTimer(0.1f).Timeout += () =>
		{
			FinalLevel.Instance.AnimationPlayer.Play("BossDeath");
			BossExplosion1Sound.Play();
			BossExplosion2Sound.Play();
			GameManager.Instance.BossExplosionSound.Play();
			FinalLevel.Instance.Player3D.ProcessMode = ProcessModeEnum.Disabled;
			SpeedRunTimer.Instance.StopTimer();
			GlitchTimer.Stop();
			GameManager.SaveData.HaveFinishTheGameAtLeastOneTime = true;
			GameManager.SaveData.Save();
		};
	}

	public void HealVirus()
	{
		if (Hp < MaxHp)
		{
			Hp++;
			UpdateHealthBar();
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
