public partial class WindowPlatform : FloatWindow
{
	public override void _Ready()
	{
		Unfocusable = false;
		base._Ready();
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void GrabWindowFocus()
	{
		base.GrabWindowFocus();
	}
}
