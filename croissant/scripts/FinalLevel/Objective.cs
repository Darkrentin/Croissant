using Godot;
using System;
using System.Collections.Generic;

public partial class Objective : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public CollisionShape3D CollisionShape;
	[Export] public OmniLight3D Light;
	[Export] public AnimationPlayer AnimationPlayer;
	public AnimationPlayer LocalAnimationPlayer;
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
			QueueFree();
		};
		AddChild(timer);
		LocalAnimationPlayer = (AnimationPlayer)GetNode("AnimationPlayer");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_isBreaking && AnimationPlayer.IsPlaying())
		{
			AnimationPlayer.Advance((float)delta);
		}
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
		
		LocalAnimationPlayer.Play("Break");

		FinalLevel.Instance.ObjectiveDestroy();

		timer.Start();
	}
}
