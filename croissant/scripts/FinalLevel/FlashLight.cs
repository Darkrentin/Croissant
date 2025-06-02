using Godot;
using System;

public partial class FlashLight : Node3D
{
	[Export] public Area3D Area3D;
	[Export] public Sprite3D Sprite3D;
	[Export] public AudioStreamPlayer CollectSound;
	public override void _Ready()
	{
		Area3D.BodyEntered += OnBodyEntered;
	}

	public override void _Process(double delta)
	{
		Sprite3D.LookAt(FinalLevel.Instance.Player3D.GlobalPosition, Vector3.Up);
		Sprite3D.Rotation *= new Vector3(0, 1, 0);
	}

	public void OnBodyEntered(Node3D body)
	{
		if (body is Player3D player)
		{
			Visible = false;
			player.TurnOnFlashlight();
			CollectSound.Play();
			CollectSound.Finished += Delete;
		}
	}
	public void Delete()
	{
		FinalLevel.Instance.maze.FlashLight = null;
		GetParent().RemoveChild(this);
		QueueFree();
	}
}
