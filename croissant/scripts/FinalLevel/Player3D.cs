using Godot;
using System;

public partial class Player3D : CharacterBody3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public float MoveSpeed = 10.0f;
	[Export] public float RotationSpeed = 3.0f;
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

    public override void _PhysicsProcess(double delta)
    {
        float z_movement = Input.GetActionStrength("Backward") - Input.GetActionStrength("Forward");
		float rotate = Input.GetActionStrength("LeftRot") - Input.GetActionStrength("RightRot");

		Rotation = new Vector3(Rotation.X,Rotation.Y + RotationSpeed * rotate * (float)delta, Rotation.Z);
		Vector3 direction = new Vector3(0, 0, 1).Rotated(new Vector3(0, 1, 0), Rotation.Y);
		Vector3 motion = z_movement * direction * MoveSpeed * (float)delta;

		MoveAndCollide(motion);
    }

}
