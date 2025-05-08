using Godot;

public partial class MenuWindow : FloatWindow
{
	[Export] private Control Menu;
	[Export] public CheckButton FakeDesktopButton;
	[Export] public CheckButton DebugButton;
	public bool FakeDesktop = false;
	public bool DebugMode = false;

	public override void _Ready()
	{
		ProcessMode = ProcessModeEnum.Always;
		base._Ready();

		Vector2I windowSize = Lib.GetScreenSize(0.1f, 0.2f);
		SetDeferred("size", windowSize);
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - windowSize / 2;
		Visible = false;

		SetDeferred("custom_minimum_size", windowSize);
		Menu.SetDeferred("size", windowSize);

		FakeDesktopButton.Toggled += FakeDesktopButtonToggled;
		DebugButton.Toggled += DebugButtonToggled;
		FakeDesktop = GameManager.SaveData.FakeDesktop;
		DebugMode = GameManager.SaveData.DebugMode;

		FakeDesktopButton.ButtonPressed = FakeDesktop;
		DebugButton.ButtonPressed = DebugMode;
	}

	public override void _Process(double delta)
	{
		base._Process(delta);

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

		if(FakeDesktop && !MainWindow.FakeBackground.Visible)
			FakeDesktopButtonToggled(FakeDesktop);
		if(DebugMode && !MainWindow.DebugInfo.Visible)
			DebugButtonToggled(DebugMode);
	}

	public void Close()
	{
		Visible = false;
		Virus.SetPause(false);
		GetTree().Paused = false;
	}

	public void Open()
	{
		Input.MouseMode = Input.MouseModeEnum.Visible;
		Visible = true;
		ProcessMode = ProcessModeEnum.Always;
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
		if(MainWindow.FakeBackground == null)
			return;
		MainWindow.FakeBackground.Visible = FakeDesktop;
	}

	public void DebugButtonToggled(bool toggled)
	{
		DebugMode = toggled;
		MainWindow.DebugInfo.Visible = DebugMode;
	}
		
}