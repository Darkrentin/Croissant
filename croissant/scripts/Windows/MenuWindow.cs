using Godot;

public partial class MenuWindow : FloatWindow
{
	[Export] public CheckButton FakeDesktopButton;
	[Export] public CheckButton DebugButton;
	[Export] public Button StuckButton;
	[Export] public Button GrabFocusButton;
	[Export] public Slider MasterVolumeSlider;
	[Export] public Slider MusicVolumeSlider;
	[Export] public Slider SFXVolumeSlider;
	[Export] public AudioStreamPlayer MenuEnter;
	[Export] public AudioStreamPlayer MenuExit;
	[Export] public AudioStreamPlayer MenuClick;

	public bool FakeDesktop = false;
	public bool DebugMode = false;

	public Vector2I SizeWithout = new Vector2I(300, 335);
	public Vector2I SizeWith = new Vector2I(300, 380);

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		base._Ready();
		Title = "Menu";

		Vector2I windowSize = Lib.GetAspectFactor(SizeWithout);
		Size = windowSize;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - windowSize / 2;
		Visible = false;

		FakeDesktopButton.Toggled += FakeDesktopButtonToggled;
		DebugButton.Toggled += DebugButtonToggled;
		StuckButton.Pressed += StuckButtonPressed;
		GrabFocusButton.Pressed += OnGrabFocusButtonPressed;

		MasterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
		MusicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		SFXVolumeSlider.ValueChanged += OnSFXVolumeChanged;

		FakeDesktop = GameManager.SaveData.FakeDesktop;
		DebugMode = GameManager.SaveData.DebugMode;

		FakeDesktopButton.ButtonPressed = FakeDesktop;
		DebugButton.ButtonPressed = DebugMode;

		// Set initial volume slider values
		MasterVolumeSlider.Value = GameManager.SaveData.MainVolume;
		MusicVolumeSlider.Value = GameManager.SaveData.MusicVolume;
		SFXVolumeSlider.Value = GameManager.SaveData.SfxVolume;
		OnMasterVolumeChanged(MasterVolumeSlider.Value);
		OnMusicVolumeChanged(MusicVolumeSlider.Value);
		OnSFXVolumeChanged(SFXVolumeSlider.Value);

		Minimizable = true;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

		if (GameManager.State == GameManager.GameState.Level3Buffer && StuckButton.Visible == false)
		{
			StuckButton.Visible = true;
			Size = Lib.GetAspectFactor(SizeWith);
		}
		else if (GameManager.State != GameManager.GameState.Level3Buffer && StuckButton.Visible == true)
		{
			StuckButton.Visible = false;
			Size = Lib.GetAspectFactor(SizeWithout);
		}

		if (Input.IsActionJustPressed("Exit"))
		{
			if (Visible)
				Close();
			else
				Open();
		}

		if (Mode is ModeEnum.Minimized)
		{
			Mode = ModeEnum.Windowed;
		}

		if (FakeDesktop && !MainWindow.FakeBackground.Visible)
			FakeDesktopButtonToggled(FakeDesktop);
		if (DebugMode && !MainWindow.DebugInfo.Visible)
			DebugButtonToggled(DebugMode);
	}

	public void Close()
	{
		GameManager.Instance.FocusAllWindows();
		GetTree().CreateTimer(0.01f).Timeout += () =>
		{
			GameManager.StartRefocusAllWindows();
		};
		MenuExit.Play();
		Visible = false;
		if (GameManager.virus != null)
			Virus.SetPause(false);
		GetTree().Paused = false;
	}

	public void Open()
	{
		if (GameManager.State == GameManager.GameState.ScreenScaleScreenBuffer)
		{
			return;
		}
		GameManager.Instance.UnfocuseAllWindows();
		Unfocusable = false;
		MenuEnter.Play();
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Visible = true;
		ProcessMode = ProcessModeEnum.Always;
		if (GameManager.virus != null)
			Virus.SetPause(true);
		GetTree().Paused = true;
	}

	public void _on_quit_button_pressed()
	{
		MenuClick.Play();
		GameManager.SaveData.Save();
		GetTree().Quit();
	}

	public override void OnClose()
	{
		////Lib.Print("OnClose MenuWindow");
		Close();
	}

	public void FakeDesktopButtonToggled(bool toggled)
	{
		MenuClick.Play();
		FakeDesktop = toggled;
		if (MainWindow.FakeBackground == null)
			return;
		MainWindow.FakeBackground.Visible = FakeDesktop;
	}

	public void DebugButtonToggled(bool toggled)
	{
		MenuClick.Play();
		DebugMode = toggled;
		if (MainWindow.DebugInfo == null)
			return;
		MainWindow.DebugInfo.Visible = DebugMode;
	}

	public void StuckButtonPressed()
	{
		MenuClick.Play();
		Close();
		Level3.Instance.TransitionStuck();
	}

	private void OnMasterVolumeChanged(double value)
	{
		SetBusVolume("Master", value);
	}

	private void OnMusicVolumeChanged(double value)
	{
		SetBusVolume("Music", value);
	}

	private void OnSFXVolumeChanged(double value)
	{
		SetBusVolume("SFX", value);
	}

	private void SetBusVolume(string busName, double value)
	{
		float volumePercent = (float)value / 100.0f;
		float volumeDb;

		if (volumePercent <= 0.0f)
		{
			volumeDb = -80.0f;
		}
		else
		{
			volumeDb = Mathf.LinearToDb(volumePercent);
		}

		int busIndex = AudioServer.GetBusIndex(busName);
		if (busIndex != -1)
		{
			AudioServer.SetBusVolumeDb(busIndex, volumeDb);
		}
	}

	private void OnGrabFocusButtonPressed()
	{
		MenuClick.Play();
		Close();
	}
}
