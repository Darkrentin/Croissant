using Godot;
using System.Collections.Generic;
using System.Numerics;

public partial class Objective : StaticBody3D
{
	public static List<Objective> ObjectiveList = new List<Objective>();
	[Export] public CollisionShape3D CollisionShape;
	[Export] public OmniLight3D Light;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public RigidBody3D[] PartList;
	[Export] public Color PixelColor;
	[Export] public MeshInstance3D Center;
	[Export] public ObjectiveExplosion Explosion;
	public bool _isBreaking = false;
	public Timer timer;
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
		AnimationPlayer.AnimationFinished += AnimationFinished;
		ApplyEmissionColor();
	}

	public void ApplyEmissionColor()
	{
		Center.MaterialOverride = new StandardMaterial3D();
		Center.MaterialOverride.Set("emission_enabled", true);
		Center.MaterialOverride.Set("albedo_color", PixelColor);
		Center.MaterialOverride.Set("emission", PixelColor);

		Explosion.MaterialOverride = new StandardMaterial3D();
		Explosion.MaterialOverride.Set("emission_enabled", true);
		Explosion.MaterialOverride.Set("albedo_color", PixelColor);
		Explosion.MaterialOverride.Set("emission", PixelColor);
	}

	public void AnimationFinished(StringName name)
	{
		if (name == "RESET")
			AnimationPlayer.Play("Idle");
	}

	public void Break()
	{
		Godot.Vector3 tmp = (GlobalPosition - FinalLevel.Instance.Player3D.GlobalPosition).Normalized();
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
			part.ApplyImpulse((tmp + part.Position).Normalized() * impulse);
		}

		//AnimationPlayer.CallbackModeProcess = AnimationPlayer.AnimationCallbackModeProcess.Idle;
		Explosion.ProcessMaterial.Set("gravity", tmp * 30);
		Explosion.Emitting = true;
		Explosion.Timer.Start();
		AnimationPlayer.Play("Break");
		FinalLevel.Instance.ObjectiveDestroy();
		timer.Start();
	}

	public void ResetOtherObjectives()
	{
		foreach (Objective obj in ObjectiveList)
			obj.AnimationPlayer.Play("RESET");
	}
}