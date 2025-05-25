using Godot;

public partial class MenuWindow : FloatWindow
{
	[Export] private Control Menu;
	[Export] public CheckButton FakeDesktopButton;
	[Export] public CheckButton DebugButton;
	[Export] public Button StuckButton;
	[Export] public Slider MasterVolumeSlider;
	[Export] public Slider MusicVolumeSlider;
	[Export] public Slider SFXVolumeSlider;
	public bool FakeDesktop = false;
	public bool DebugMode = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		base._Ready();

		Vector2I windowSize = Lib.GetAspectFactor(Size);
		Size = windowSize;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - windowSize / 2;
		Visible = false;

		FakeDesktopButton.Toggled += FakeDesktopButtonToggled;
		DebugButton.Toggled += DebugButtonToggled;
		StuckButton.Pressed += StuckButtonPressed;
		
		// Connect all volume sliders
		MasterVolumeSlider.ValueChanged += OnMasterVolumeChanged;
		MusicVolumeSlider.ValueChanged += OnMusicVolumeChanged;
		SFXVolumeSlider.ValueChanged += OnSFXVolumeChanged;

		FakeDesktop = GameManager.SaveData.FakeDesktop;
		DebugMode = GameManager.SaveData.DebugMode;

		FakeDesktopButton.ButtonPressed = FakeDesktop;
		DebugButton.ButtonPressed = DebugMode;
		
		// Set initial volume slider values (load from save data or default to 100)
		MasterVolumeSlider.Value = GameManager.SaveData.MainVolume;
		MusicVolumeSlider.Value = GameManager.SaveData.MusicVolume;
		SFXVolumeSlider.Value = GameManager.SaveData.SfxVolume;
		
		// Apply the initial volumes
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
		}
		else if (GameManager.State!= GameManager.GameState.Level3Buffer && StuckButton.Visible == true)
		{
			StuckButton.Visible = false;
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
			////Lib.Print("Close Minimized");
			Close();
		}

		if (FakeDesktop && !MainWindow.FakeBackground.Visible)
			FakeDesktopButtonToggled(FakeDesktop);
		if (DebugMode && !MainWindow.DebugInfo.Visible)
			DebugButtonToggled(DebugMode);
	}

	public void Close()
	{
		Visible = false;
		if (GameManager.virus != null)
			Virus.SetPause(false);
		GetTree().Paused = false;
	}

	public void Open()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Visible = true;
		ProcessMode = ProcessModeEnum.Always;
		if (GameManager.virus != null)
			Virus.SetPause(true);
		GetTree().Paused = true;

	}

	public void _on_quit_button_pressed()
	{
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
		FakeDesktop = toggled;
		if (MainWindow.FakeBackground == null)
			return;
		MainWindow.FakeBackground.Visible = FakeDesktop;
	}

	public void DebugButtonToggled(bool toggled)
	{
		DebugMode = toggled;
		if (MainWindow.DebugInfo == null)
			return;
		MainWindow.DebugInfo.Visible = DebugMode;
	}

	public void StuckButtonPressed()
	{
		Close();
		Level3.Instance.Transition(-1);
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
		// Convert slider value (0-100) to decibels
		float volumePercent = (float)value / 100.0f;
		float volumeDb;
		
		if (volumePercent <= 0.0f)
		{
			// Mute if slider is at 0
			volumeDb = -80.0f;
		}
		else
		{
			// Convert percentage to dB (logarithmic scale)
			volumeDb = Mathf.LinearToDb(volumePercent);
		}
		
		// Get bus index and apply volume
		int busIndex = AudioServer.GetBusIndex(busName);
		if (busIndex != -1)
		{
			AudioServer.SetBusVolumeDb(busIndex, volumeDb);
		}
		else
		{
			GD.PrintErr($"Audio bus '{busName}' not found!");
		}
	}

}
