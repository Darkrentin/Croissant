using Godot;

public partial class Portal : Area2D
{
	[Export] public int NextSceneId = 1;
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public bool PortalToSpawn = false;
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
		if (isChangingScene)
			return;

		if (body is PlayerCharacter player)
		{
			GD.Print("OnBoddyEntered");
			player.isInvincible = true;
			player.isDead = true;
			Level3.Instance.Transition(this.NextSceneId, this);
			
		}
	}

	public void OnVisibilityChange()
	{
		if (Visible)
		{
			timer.Start();
			isChangingScene = true;
		}
	}
}
