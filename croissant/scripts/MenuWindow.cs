using Godot;
using System;

public partial class MenuWindow : FloatWindow
{
	[Export] private Control Menu;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Size = Lib.GetScreenSize(0.1f,0.2f);
		Position = Lib.GetScreenPosition(0.5f,0.5f) - Size/2;
		Visible = false;
		Menu.Size = Size;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);


		if(Input.IsActionJustPressed("Exit"))
		{
			if(Visible)
			{
				Close();
				GD.Print("Close");
			}
			else
			{
				Open();
			}
		}

		if(Mode is ModeEnum.Minimized)
		{
			Mode = ModeEnum.Windowed;
			GD.Print("Close Minimized");
			Close();
		}
	}

	private void Close()
	{
		Visible = false;
		GetTree().Paused = false;
	}

	private void Open()
	{
		Visible = true;
		ProcessMode = ProcessModeEnum.Always;
		GetTree().Paused = true;
	}

	public void _on_quit_button_pressed()
	{
		GetTree().Quit();
	}

	public override void OnClose()
	{
		GD.Print("OnClose MenuWindow");
		Close();
	}
}
