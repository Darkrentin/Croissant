using Godot;
using System;
using System.Collections.Generic;

public partial class Objective : StaticBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public CollisionShape3D CollisionShape;
	[Export] public OmniLight3D Light;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public RigidBody3D[] PartList;
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
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void Break()
	{
		Lib.Print("Break");
		RemoveChild(CollisionShape);
		CollisionShape.QueueFree();
		RemoveChild(Light);
		Light.QueueFree();

		const float impulse = 5f;
		foreach (var part in PartList)
		{
			part.Freeze = false;
			part.ApplyImpulse(((GlobalPosition-FinalLevel.Instance.Player3D.GlobalPosition).Normalized() + part.Position).Normalized()*impulse);
		}
		AnimationPlayer.Play("Break");
		timer.Start();
	}
}
