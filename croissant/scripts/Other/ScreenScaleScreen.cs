using Godot;

public partial class ScreenScaleScreen : Window
{
	[Export] public RichTextLabel WarningLabel;
	[Export] public Button AcceptButton;
	[Export] public Button RefuseButton;

	public override void _Ready()
	{
		Title = "Warning";
		ProcessMode = ProcessModeEnum.Always;
		CloseRequested += () => GetTree().Quit();

		Vector2I windowSize = Lib.GetAspectFactor(Size);
		Size = windowSize;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - windowSize / 2;
		Visible = true;

		WarningLabel.Text = "Due to Windows limitations about window widths, you must play the game at [color=RED][b]100%[/b][/color] screen scale instead of your actual [color=RED][b]" + GameManager.ScreenScale * 100 + "%[/b][/color].";

		AcceptButton.Pressed += OnAcceptButtonPressed;
		RefuseButton.Pressed += OnRefuseButtonPressed;
	}

	public void OnAcceptButtonPressed()
	{
		string uri = "ms-settings:display";
		Error result = OS.ShellOpen(uri);
		if (result == Error.Ok)
		{
			GetTree().Quit();
		}
		else
		{
			GetTree().CreateTimer(0.5f).Timeout += () => GetTree().Quit();
		}
	}

	public void OnRefuseButtonPressed()
	{
		GameManager.State = GameManager.GameState.ParticulesPreload;
		QueueFree();
	}
}
