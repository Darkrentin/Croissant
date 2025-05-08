using Godot;
using System;

public partial class Lamp : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public int RenderDistance = 10;
	[Export] public OmniLight3D Light;
	public Timer timer;
	public override void _Ready()
	{
		timer = new Timer();
		timer.WaitTime = Lib.GetRandomNormal(0.05f, 0.1f);
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			timer.WaitTime = Lib.GetRandomNormal(0.05f, 0.1f);
			Light.LightEnergy = Lib.GetRandomNormal(0.05f, 0.3f);
			timer.Start();
		};
		AddChild(timer);
		timer.Start();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public override void _PhysicsProcess(double delta)
	{
		if(GlobalPosition.DistanceTo(FinalLevel.Instance.Player3D.GlobalPosition)<RenderDistance)
		{
			Visible = true;

		}
		else
		{
			Visible = false;
		}
	}	
}
