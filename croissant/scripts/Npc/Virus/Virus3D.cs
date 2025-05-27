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

	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] Sprite2D HealthBar;
	public static Virus3D Instance;

	public int Hp = 10;



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

		AnimationPlayer.AnimationFinished += OnAnimationFinished;
		HealthBar.Frame = 10 - Hp;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//UpdateVirusMovement(delta);

	}

	public void UpdateVirusMovement(double delta)
	{
		LookAt(FinalLevel.Instance.Player3D.GlobalPosition, Vector3.Up);
		Rotation *= new Vector3(0, 1, 0);
		Rotation += new Vector3(0, (float)Math.PI, 0);
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
	}

	public void TakeDamage()
	{
		// Handle damage logic here
		// For example, reduce health or trigger an animation
		StartGlitch();
		//Rotate(Lib.rand.Next(-180, 180), 60f, BossLevel.Instance.LaunchFloppyDisk);
		BossLevel.Instance.LaunchNFloppyDiskAndRotate(15, 60f,60f, 0.5f);
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

}
