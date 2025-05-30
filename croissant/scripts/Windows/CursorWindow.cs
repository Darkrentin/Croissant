using Godot;

public partial class CursorWindow : FloatWindow
{
	[Export] public AudioStreamPlayer DeathSound;
	[Export] public AudioStreamPlayer RecycleSound;
	public CollisionShape2D collision;
	public Area2D area;
	public bool Freeze = false;
	public bool Invisible = false;
	public Timer FreezeTimer;
	[Export] public AnimationPlayer animationPlayer;
	private Node dotContainer;

	public override void _Ready()
	{
		base._Ready();
		Size = Lib.GetAspectFactor(Size);
		SetWindowPosition(Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2);
		FreezeTimer = new Timer();
		FreezeTimer.OneShot = true;
		FreezeTimer.Timeout += FreezFrameStop;
		AddChild(FreezeTimer);

		dotContainer = new Node();
		GameManager.MainWindow.AddChild(dotContainer);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (!Freeze && Input.IsActionJustPressed("LeftClick"))
		{
			ClearAllDots();

			var clickParticles = States.SceneLoader.ClickParticlesScene.Instantiate<ClickParticles>();
			clickParticles.GlobalPosition = Lib.GetCursorPosition();
			GameManager.MainWindow.AddChild(clickParticles);
			clickParticles.Emitting = true;
			StartExponentialTransition(Lib.GetCursorPosition() - Size / 2, 0.9f, reset: true);

			var windowCenter = Position + Size / 2;
			var cursorPos = Lib.GetCursorPosition();
			var distance = windowCenter.DistanceTo(cursorPos);
			var direction = ((Vector2)(cursorPos - windowCenter)).Normalized();
			var dotSpacing = 20.0f;
			var dotRadius = 2.0f;
			var numDots = (int)(distance / dotSpacing);

			for (int i = 0; i < numDots; i++)
			{
				var dotPosition = windowCenter + direction * (i * dotSpacing);
				var dot = new ColorRect();
				dot.Size = new Vector2(dotRadius * 2, dotRadius * 2);
				dot.Position = dotPosition - dot.Size / 2;
				dot.Color = Colors.White;
				dotContainer.AddChild(dot);
				var tween = CreateTween();
				var distanceFromStart = i * dotSpacing;
				var progressRatio = distanceFromStart / distance;
				var timeToReach = progressRatio == 0 ? 0 : (-Mathf.Log(1 - progressRatio) / Mathf.Log(2) / 10) * 0.9f;
				tween.TweenInterval(timeToReach);
				tween.TweenCallback(new Callable(dot, "queue_free"));
			}
		}
	}

	private void ClearAllDots()
	{
		foreach (Node child in dotContainer.GetChildren())
		{
			child.QueueFree();
		}
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
		RecycleSound.Play();
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
