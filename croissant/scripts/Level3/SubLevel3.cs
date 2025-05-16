using Godot;
using System;

public partial class SubLevel3 : Node2D
{
	[Export] public FloatWindow[] windows;
	public bool CurrentLevel = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void HideSubLevel()
	{
		foreach (var window in windows)
		{
			window.Hide();
		}

		Position = Lib.GetAspectFactor(new Vector2I(1920, 1080))*2;
		CurrentLevel = false;
		Visible = false;
	}

	public void ShowSubLevel()
	{
		foreach (var window in windows)
		{
			window.Show();
		}

		Position = new Vector2(0, 0);
		CurrentLevel = true;
		Visible = true;
	}
}
