using Godot;
using System;

public partial class SubLevel3 : Node2D
{
	[Export] public FloatWindow[] windows;
	[Export] public PortalExit portalExit;
	public bool CurrentLevel = false;
	public void HideSubLevel()
	{
		Position = Lib.GetAspectFactor(new Vector2I(1920, 1080)) * 2;
		Visible = false;
		// Use call_deferred to avoid physics conflicts when hiding windows
		CallDeferred(nameof(HideWindowsDeferred));
		CurrentLevel = false;
	}

	private void HideWindowsDeferred()
	{
		foreach (var window in windows)
			window.Hide();
	}

	public void ShowSubLevel()
	{
		// Use call_deferred to avoid physics conflicts when showing windows
		CallDeferred(nameof(ShowWindowsDeferred));
		Position = new Vector2(0, 0);
		CurrentLevel = true;
		Visible = true;
	}

	private void ShowWindowsDeferred()
	{
		foreach (var window in windows)
			window.Show();
	}
}
