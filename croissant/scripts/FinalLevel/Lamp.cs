using Godot;
using System;

public partial class Lamp : Node3D
{
	[Export] public int RenderDistance = 10;
	[Export] public OmniLight3D Light;
	[Export] public AudioStreamPlayer3D LampSound;
	public Timer timer;
	public override void _Ready()
	{
		timer = new Timer();
		timer.WaitTime = Lib.GetRandomNormal(0.05f, 0.1f);
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			timer.WaitTime = Lib.GetRandomNormal(0.05f, 0.1f);
			float intensity = Lib.GetRandomNormal(0.05f, 0.3f);
			Light.LightEnergy = intensity;
			
			// Adjust sound volume based on light intensity
			if (LampSound != null)
			{
				// Scale volume between 0.1 and 1.0 based on light intensity
				LampSound.VolumeDb = Mathf.LinearToDb(Mathf.Remap(intensity, 0.05f, 0.3f, 0.1f, 1.0f));
				
				// Make sure sound is playing
				if (!LampSound.Playing)
				{
					LampSound.Play();
				}
			}
			
			timer.Start();
		};
		AddChild(timer);
		timer.Start();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (GlobalPosition.DistanceSquaredTo(FinalLevel.Instance.Player3D.GlobalPosition) < (RenderDistance * RenderDistance))
			Visible = true;
		else
			Visible = false;
	}
}
