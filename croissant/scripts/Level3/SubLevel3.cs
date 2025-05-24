using Godot;
using System;

public partial class SubLevel3 : Node2D
{
	[Export] public FloatWindow[] windows;
	public bool CurrentLevel = false;

	public void HideSubLevel()
	{
		Position = Lib.GetAspectFactor(new Vector2I(1920, 1080)) * 2;
		Visible = false;
		foreach (var window in windows)
			window.Hide();

		CurrentLevel = false;
	}

	public void ShowSubLevel()
	{
		foreach (var window in windows)
			window.Show();

		Position = new Vector2(0, 0);
		CurrentLevel = true;
		Visible = true;
	}
}
