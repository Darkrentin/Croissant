using Godot;

public partial class MeltShader : ColorRect
{
	private bool _melting = false;
	private double _timer = 0.0;

	[Export] public double XResolution { get; set; } = 100.0;
	[Export] public double MaxOffset { get; set; } = 2.0;
	[Export] public double MeltSpeed { get; set; } = 0.02;

	public override void _Ready()
	{
		Hide();
	}

	public override void _Process(double delta)
	{
		if (_melting)
		{
			_timer += MeltSpeed; // Assuming MeltSpeed is not delta-dependent as per GDScript
			if (Material is ShaderMaterial shaderMaterial)
				shaderMaterial.SetShaderParameter("timer", _timer);
		}
	}

	// Call this before transitioning, creates a copy of the screen texture so changes
	// can be made underneath before melting to show the new screen.
	public void GenerateOffsets()
	{
		var offsets = new Godot.Collections.Array<float>();
		for (int i = 0; i < (int)XResolution; i++)

			offsets.Add(Lib.GetRandomNormal(1.0f, (float)MaxOffset));

		if (Material is ShaderMaterial SM)
		{
			SM.SetShaderParameter("y_offsets", offsets);
			Image img = GetViewport().GetTexture().GetImage();
			ImageTexture tex = ImageTexture.CreateFromImage(img);
			SM.SetShaderParameter("melt_tex", tex);
		}

		Show();
	}

	public void Transition()
	{
		if (Material is ShaderMaterial SM)
			SM.SetShaderParameter("melting", true);
		_melting = true;
	}
}