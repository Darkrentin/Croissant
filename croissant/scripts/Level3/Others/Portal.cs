using System.Runtime.Intrinsics.Wasm;
using Godot;

public partial class Portal : Area2D
{
	[Export] public int NextSceneId = 1;
	public Timer timer;
	public bool isChangingScene = false;
	public override void _Ready()
	{
		// Connect the body entered signal to the OnBoddyEntered method
		BodyEntered += OnBoddyEntered;
		timer = new Timer();
		timer.WaitTime = 1.1f;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			isChangingScene = false;
			timer.Stop();
		};
		VisibilityChanged += OnVisibilityChange;
		AddChild(timer);

	}

	public void OnBoddyEntered(Node body)
	{
		if(isChangingScene)
			return;
		
		if (body is PlayerCharacter player)
		{
			GD.Print("OnBoddyEntered");
			Level3.Instance.Transition(this);
			
			
		}
	}

	public void OnVisibilityChange()
	{
		if(Visible)
		{
			timer.Start();
			isChangingScene = true;
		}
	}

	

}
