using Godot;
using System;
using System.Collections.Generic;

public partial class Objective : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	public static List<Objective> ObjectiveList = new List<Objective>();
	[Export] public CollisionShape3D CollisionShape;
	[Export] public OmniLight3D Light;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public RigidBody3D[] PartList;
	public bool _isBreaking = false;
	public Timer timer;
	public int PartCount = 8;
	public override void _Ready()
	{
		timer = new Timer();
		timer.WaitTime = 1f;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			ObjectiveList.Remove(this);
			ResetOtherObjectives();
			QueueFree();
		};
		ObjectiveList.Add(this);
		AddChild(timer);
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Break()
	{
		_isBreaking = true;
		Lib.Print("Break");
		RemoveChild(CollisionShape);
		CollisionShape.QueueFree();
		GetNode<Node3D>("Obj").RemoveChild(Light);
		Light.QueueFree();

		const float impulse = 5f;
		foreach (var part in PartList)
		{
			part.Freeze = false;
			part.ApplyImpulse(((GlobalPosition-FinalLevel.Instance.Player3D.GlobalPosition).Normalized() + part.Position).Normalized()*impulse);
		}
		
		//AnimationPlayer.CallbackModeProcess = AnimationPlayer.AnimationCallbackModeProcess.Idle;
		
		AnimationPlayer.Play("Break");

		FinalLevel.Instance.ObjectiveDestroy();

		timer.Start();
	}

	public void ResetOtherObjectives()
	{
		foreach (var obj in ObjectiveList)
		{
			obj.AnimationPlayer.Play("RESET");
		}
	}
}
