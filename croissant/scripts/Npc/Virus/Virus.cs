using Godot;

public partial class Virus : Npc
{
	[Export] Camera3D Camera;
	[Export] Node3D Computer;
	[Export] Node2D Eye;
	[Export] Node2D EyeBrow;
	[Export] Node2D black1;
	[Export] Node2D black2;
	[Export] Vector2 MaxRotation = new Vector2(0.2f, 0.2f);
	[Export] Vector2 MaxEyeDistance = new Vector2(0.1f, 0.1f);
	[Export] float RotationSmoothing = 5;
	[Export] public Control ExportPause { get => Pause; set => Pause = value; }
	[Export] public Timer BlinkTimer;
	[Export] public AnimationPlayer AnimationScale;

	public Vector2I CenterOfScreen = new Vector2I(600 / 2, 480 / 2);
	public Vector3 targetRotation;
	public static Control Pause;
	public bool On = false;
	public Node2D VirusSplash;
	public Timer SplashTimer;

	public override void _Ready()
	{
		Size = new Vector2I(335, 400);
		Size = Lib.GetAspectFactor(Size);
		base._Ready();

		BlinkTimer.Timeout += Blink;
		BlinkTimer.WaitTime = 5f;
		BlinkTimer.Start();

		AnimationScreen.Travel("Off");

		SplashTimer = new Timer();
		SplashTimer.OneShot = true;
		SplashTimer.Timeout += () =>
		{
			GameManager.GameRoot.RemoveChild(VirusSplash);
			VirusSplash.QueueFree();
			VirusSplash = null;
		};
		SplashTimer.WaitTime = 1f;
		AddChild(SplashTimer);
	}

	public override void InitNpc()
	{
		base.InitNpc();
	}

	public override void DialogueFinished(string name)
	{
		//Lib.Print(name);
		switch (name)
		{
			case "1":
				GameManager.State = GameManager.GameState.TutoBuffer;
				StartExponentialTransition(GameManager.ScreenSize - Size, 1f);
				GameManager.virus.Dialogue.Visible = false;
				break;
			case "sleep":
				AnimationScreen.Travel("Idle");
				On = true;
				Dialogue.StartDialogue(NpcName, "1");
				break;
			case "tutoEnd":
				Dialogue.LockSkip = false;
				GameManager.State = GameManager.GameState.Level1;
				break;
			case "Virus/Helper Dialogue 1":
				GetTree().CreateTimer(0.1f).Timeout += () =>
				{
					GameManager.helper.Dialogue.StartDialogue("Helper", "Virus/Helper Dialogue 2");
				};
				break;
			case "Virus/Helper Dialogue 3":
				GameManager.State = GameManager.GameState.Level2;
				break;
			case "EndLvl2":
				GameManager.helper.ShowNpc(GameManager.helper.LeftDown);
				HideNpc(1);
				GameManager.helper.DialogueToPlayAfterTransition = "EndLvl2";
				//remove the virus
				GameManager.virus.GetTree().CreateTimer(1f).Timeout += () =>
				{
					GameManager.GameRoot.RemoveChild(GameManager.virus);
					GameManager.virus.QueueFree();
					GameManager.virus = null;
				};
				break;
		}
		Lib.Print("Virus");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (On)
			UpdateModelRotation(delta);
		if (On && !OnScreen())
		{
			On = false;
		}
		if (!On && OnScreen() && AnimationScreen.GetCurrentNode() != "Off")
		{
			On = true;
		}
		// Movement Fiesta test
		//StartExponentialTransition(Lib.GetScreenPosition(Lib.GetRandomNormal(0, 1), Lib.GetRandomNormal(0, 1)), 1f);
	}

	// Makes the virus look toward the mouse
	private void UpdateModelRotation(double delta)
	{
		Vector2I cursorPosition = Lib.GetCursorPosition();
		Vector2I centerPosition = Position + Size / 2;
		Vector2I relativePosition = centerPosition - cursorPosition;

		float normalizedX = relativePosition.X / (GameManager.ScreenSize.X / 2.0f);
		float normalizedY = relativePosition.Y / (GameManager.ScreenSize.Y / 2.0f);

		normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);
		normalizedY = Mathf.Clamp(normalizedY, -1.0f, 1.0f);

		float rotationY = -normalizedX * MaxRotation.Y; // Negative because right is positive X but negative Y rotation
		float rotationX = -normalizedY * MaxRotation.X;   // Negative because down is positive Y but negative X rotation

		float positionX = -normalizedX * MaxEyeDistance.X;
		float positionY = -normalizedY * MaxEyeDistance.Y;

		Eye.Position = CenterOfScreen + new Vector2(positionX, positionY);
		EyeBrow.Position = CenterOfScreen + new Vector2(positionX, positionY) - new Vector2(0, 140);

		targetRotation = new Vector3(rotationX, rotationY, Computer.Rotation.Z);

		Computer.Rotation = Computer.Rotation.Lerp(targetRotation, (float)delta * RotationSmoothing);
		const float PosMultiplier = 4f;
		black1.Position = new Vector2(positionX, positionY) * PosMultiplier;
		black2.Position = new Vector2(positionX, positionY) * PosMultiplier;
	}

	public static void SetPause(bool Visible)
	{
		Pause.Visible = Visible;
	}

	public override void TransitionFinished()
	{
		//Lib.Print("Transition Finished");
		if (GameManager.State == GameManager.GameState.IntroVirusBuffer)
		{
			GameManager.State = GameManager.GameState.Dialogue1;
			GrabFocus();
		}
		if (GameManager.State == GameManager.GameState.TutoBuffer)
		{
			GameManager.State = GameManager.GameState.VirusTuto;
			GrabFocus();
		}
		if (DialogueToPlayAfterTransition == "Virus/Helper Dialogue 1")
		{
			Splash();
		}
		base.TransitionFinished();
	}

	public void Blink()
	{
		AnimationScale.Play("Blink");
		BlinkTimer.WaitTime = Lib.GetRandomNormal(3f, 6f);
		BlinkTimer.Start();
	}

	public void Splash()
	{
		VirusSplash = States.VirusSplashScene.Instantiate<Node2D>();
		VirusSplash.Position = GameManager.virus.Position + new Vector2I(GameManager.virus.Size.X / 2, (int)(GameManager.virus.Size.Y * 0.9f));
		GameManager.GameRoot.AddChild(VirusSplash);
		VirusSplash.GetNode<CpuParticles2D>("VirusSplashLeft").Emitting = true;
		VirusSplash.GetNode<CpuParticles2D>("VirusSplashRight").Emitting = true;
		SplashTimer.Start();
	}
}
