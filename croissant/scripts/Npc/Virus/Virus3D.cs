using Godot;
using System;

public partial class Virus3D : Node3D
{
	// Called when the node enters the scene tree for the first time.
	public float MapWidth = 20;
	public float MaxRotation = 1f;
	public float RotationSmoothing = 5;

	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;
	public override void _Ready()
	{
		AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
		AnimationScreen.Travel("Angry");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		UpdateVirusMovement(delta);
	}

	public void UpdateVirusMovement(double delta)
	{
		LookAt(FinalLevel.Instance.Player3D.GlobalPosition, Vector3.Up);
		Rotation*= new Vector3(0, 1, 0);
		Rotation+=new Vector3(0, (float)Math.PI, 0);
	}
}
