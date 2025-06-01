using Godot;

public partial class Credits : FloatWindow
{
	public override void _Ready()
	{
		Title = "Credits";
		CloseRequested += () => Visible = false;

		Vector2I windowSize = Lib.GetAspectFactor(Size);
		Size = windowSize;
		Position = Lib.GetScreenPosition(0.5f, 0.5f) - windowSize / 2;
		Visible = true;
	}

	public void _on_rich_text_label_meta_clicked(string meta)
	{
		OS.ShellOpen(meta);
	}
}
