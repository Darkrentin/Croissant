using Godot;
using System;

public partial class Virus : FloatWindow
{
	[Export] Camera3D Camera;
	[Export] Node3D Computer;

	[Export] public AnimationPlayer AnimationScale;
	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;
	[Export] Node2D Eye;
	[Export] Node2D EyeBrow;
	public Vector2I CenterOfScreen = new Vector2I(600 / 2, 480 / 2);

	[Export] Vector2 MaxRotation = new Vector2(0.2f, 0.2f);
	[Export] Vector2 MaxEyeDistance = new Vector2(0.1f, 0.1f);
	[Export] float RotationSmoothing = 5;
	public Vector3 targetRotation;
	[Export] public DialogueWindow dialogue;
	public static Control Pause;
	[Export] public Control ExportPause { get => Pause; set => Pause = value; }
	[Export] public Timer BlinkTimer;
	Vector2I screenSize = DisplayServer.ScreenGetSize();

	public bool On = false;

	public override void _Ready()
	{
		base._Ready();
		AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		AnimationScreen.Travel("Off");
		Size = new Vector2I(335, 400);
		//Size *= GameManager.ScreenSize/ new Vector2I(1920, 1080);
		Size = (Vector2I)Lib.GetAspectFactor(Size);
		dialogue.PlaceDialogueWindow();
		dialogue.OnDialogueFinished += DialogueFinished;

		BlinkTimer.Timeout += Blink;
		BlinkTimer.WaitTime = 5f;
		BlinkTimer.Start();
	}

	public void DialogueFinished(string name)
	{
		Lib.Print(name);
		switch (name)
		{
			case "1":
				GameManager.State = GameManager.GameState.TutoBuffer;
				StartExponentialTransition(GameManager.ScreenSize - Size, 1f);
				dialogue.PlaceDialogueWindow();
				break;
			case "sleep":
				AnimationScreen.Travel("Idle");
				On = true;
				dialogue.StartDialogue("Virus", "1");
				break;
			case "tutoEnd":
				GameManager.State = GameManager.GameState.Level1;
				break;
		}

	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (On)
			UpdateModelRotation(delta);
		if (Input.IsActionJustPressed("LeftClick"))
			_on_button_pressed();
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
	}

	public void _on_button_pressed()
	{
		Lib.Print("Virus Clicked");
		AnimationScale.Play("Hop");
		if (dialogue.isDialogue)
			dialogue.NextLine();
	}

	public static void SetPause(bool Visible)
	{
		if (Pause != null)
			Pause.Visible = Visible;
	}

	public override void TransitionFinished()
	{
		Lib.Print("Transition Finished");
		if (GameManager.State == GameManager.GameState.IntroVirusBuffer)
		{
			GameManager.State = GameManager.GameState.VirusDialogue1;
			GrabFocus();
		}
		if (GameManager.State == GameManager.GameState.TutoBuffer)
		{
			GameManager.State = GameManager.GameState.VirusTuto;
			GrabFocus();
		}
	}

	public void Blink()
	{
		AnimationScale.Play("Blink");
		BlinkTimer.WaitTime = Lib.GetRandomNormal(3f, 6f);
		BlinkTimer.Start();
	}
}
