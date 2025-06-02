using Godot;

public partial class VisualCollision : ColorRect
{
	public float duration;
	public float elapsedTime = 0f;
	public static bool End = false;

	public override void _Process(double delta)
	{
		if (elapsedTime < duration)
		{
			elapsedTime += (float)delta;
			//Color = new Color(Color.R, Color.G, Color.B, Mathf.Lerp(0f, 1f, elapsedTime / duration));
			((ShaderMaterial)Material).SetShaderParameter("col", new Color(Color.R, Color.G, Color.B, Mathf.Lerp(0f, 0.80f, elapsedTime / duration)));
			((ShaderMaterial)Material).SetShaderParameter("mult", Mathf.Lerp(4f, 0.88f, elapsedTime / duration));
		}
		if (End)
		{
			QueueFree();
		}
	}
}
