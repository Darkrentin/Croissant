using Godot;
using System;
using System.Runtime.Serialization;

public partial class FlashLight : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Area3D Area3D;
	[Export] public Sprite3D Sprite3D;
	public override void _Ready()
	{
		Area3D.BodyEntered += OnBodyEntered;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Sprite3D.LookAt(FinalLevel.Instance.Player3D.GlobalPosition, Vector3.Up);
		Sprite3D.Rotation*= new Vector3(0, 1, 0);
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body is Player3D player)
		{
			player.Flashlight.Visible = true;
			GetParent().RemoveChild(this);
			QueueFree();
		}
	}
}
