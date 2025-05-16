using Godot;

public partial class MeltShader : ColorRect
{
	private bool _melting = false;
	private double _timer = 0.0;
	private Godot.Collections.Array<float> _offsets;
	private ImageTexture _cachedTexture;

	[Export] public double XResolution { get; set; } = 100.0;
	[Export] public double MaxOffset { get; set; } = 2.0;
	[Export] public double MeltSpeed { get; set; } = 0.02;
	public float _maxYOffset;

	public override void _Ready()
	{
		Hide();
		GenerateOffsetsOnce();
	}

	public override void _Process(double delta)
	{
		if (_melting)
		{
			_timer += MeltSpeed * (float)delta * 60;
			if (Engine.GetFramesDrawn() % 2 == 0 && Material is ShaderMaterial shaderMaterial)
				shaderMaterial.SetShaderParameter("timer", _timer);
			if (_timer * _maxYOffset > 2.0f)
				ResetMelt();
		}
	}

	private void ResetMelt()
	{
		_melting = false;
		_timer = 0.0f;
		if (Material is ShaderMaterial SM)
			SM.SetShaderParameter("melting", false);
		FinalLevel.Instance.Player3D.ProcessMode = ProcessModeEnum.Always;
		Hide();
	}

	private void GenerateOffsetsOnce()
	{
		_offsets = new Godot.Collections.Array<float>();
		_maxYOffset = 0.0f;
		for (int i = 0; i < (int)XResolution; i++)
		{
			_offsets.Add(Lib.GetRandomNormal(1.0f, (float)MaxOffset));
			if (_offsets[i] > _maxYOffset)
				_maxYOffset = _offsets[i];
		}
		if (Material is ShaderMaterial SM)
			SM.SetShaderParameter("y_offsets", _offsets);
	}

	public void PrepareTransition()
	{
		if (_cachedTexture == null)
		{
			Image img = GetViewport().GetTexture().GetImage();
			_cachedTexture = ImageTexture.CreateFromImage(img);
		}
		else
		{
			Image img = GetViewport().GetTexture().GetImage();
			_cachedTexture.Update(img);
		}
		if (Material is ShaderMaterial SM)
			SM.SetShaderParameter("melt_tex", _cachedTexture);

		Show();
	}

	public void Transition()
	{
		if (Material is ShaderMaterial SM)
			SM.SetShaderParameter("melting", true);
		_melting = true;
	}
}
