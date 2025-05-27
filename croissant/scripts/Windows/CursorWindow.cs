using Godot;

public partial class CursorWindow : FloatWindow
{
	[Export] public AudioStreamPlayer DeathSound;
	public CollisionShape2D collision;
	public Area2D area;
	public bool Freeze = false;
	public bool Invisible = false;
	public Timer FreezeTimer;
	[Export] public AnimationPlayer animationPlayer;

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
			StartExponentialTransition(Lib.GetCursorPosition() - Size / 2, 0.9f, reset: true);
	}

	public override void OnClose()
	{
		base.OnClose();
		StartShake(0.2f, 10);
	}

	public void TakeDamage()
	{
		Lib.Print("CursorWindow: TakeDamage");
		if (Invisible)
			return;
		FreezFrameStart();
		animationPlayer.Play("Disolve");
		DeathSound.Play();
		
	}

	public void FreezFrameStart()
	{
		ProcessMode = ProcessModeEnum.Always;
		GameManager.MenuWindow.ProcessMode = ProcessModeEnum.Pausable;
		IsTransitioning = false;
		Freeze = true;
		GetTree().Paused = true;
		StartShake(0.8f, 10);
		FreezeTimer.WaitTime = 0.9f;
		FreezeTimer.Start();
		Invisible = true;
	}

	public void FreezFrameStop()
	{
		ProcessMode = ProcessModeEnum.Pausable;
		GameManager.MenuWindow.ProcessMode = ProcessModeEnum.Always;
		Freeze = false;
		GetTree().Paused = false;
		Level2.Instance.WaveManager.GoBackToWave();
		Invisible = false;
	}
}
