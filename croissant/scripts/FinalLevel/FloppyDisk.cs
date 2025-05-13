using Godot;
using System;

public partial class FloppyDisk : CharacterBody3D
{
	// Called when the node enters the scene tree for the first time.
    [Export]
    public float Speed { get; set; } = 1f;

	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity;
            
        LookAt(FinalLevel.Instance.Player3D.GlobalPosition, Vector3.Up);
        GlobalRotation *= new Vector3(0, 1, 0);
		GlobalRotation += new Vector3(0, (float)Math.PI, 0);

		// Calculate the direction to the player and move towards them
 
		Vector3 directionToPlayer = (FinalLevel.Instance.Player3D.GlobalPosition - GlobalPosition).Normalized();
        velocity = directionToPlayer * Speed * new Vector3(1, 0, 1);
        
        Velocity = velocity;
        MoveAndSlide();
	}
}
