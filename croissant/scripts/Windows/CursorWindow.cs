using Godot;

public partial class CursorWindow : FloatWindow
{
	public CollisionShape2D collision;
	public Area2D area;
	public bool Freeze = false;
	public Timer FreezeTimer;
	public override void _Ready()
	{
		base._Ready();
		Size = Lib.GetAspectFactor(Size);
		SetWindowPosition(Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2);
		FreezeTimer = new Timer();
		FreezeTimer.OneShot = true;
		FreezeTimer.Timeout += FreezFrameStop;
		AddChild(FreezeTimer);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Freeze && Input.IsActionJustPressed("LeftClick"))
			StartExponentialTransition(Lib.GetCursorPosition() - Size / 2, 1f, reset: true);
	}

	public override void OnClose()
	{
		base.OnClose();
		StartShake(0.2f, 10);
	}

	public void TakeDamage()
	{
		FreezFrameStart();
	}

	public void FreezFrameStart()
	{
		ProcessMode = ProcessModeEnum.Always;
		IsTransitioning = false;
		Freeze = true;
		GetTree().Paused = true;
		StartShake(0.4f, 10);
		FreezeTimer.WaitTime = 0.4f;
		FreezeTimer.Start();
	}

	public void FreezFrameStop()
	{
		ProcessMode = ProcessModeEnum.Pausable;
		Freeze = false;
		GetTree().Paused = false;
	}
}