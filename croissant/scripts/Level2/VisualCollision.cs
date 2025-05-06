using Godot;

public partial class VisualCollision : ColorRect
{
	public float duration;
	public float elapsedTime = 0f;
	public static int nbOfInstances = 0;
	public ShaderMaterial Shader;
	public int id = 0;

	public override void _Ready()
	{
		nbOfInstances++;
		id = nbOfInstances;
		Shader = ((ShaderMaterial)Material);
	}

	public override void _Process(double delta)
	{
		if (elapsedTime < duration)
		{
			elapsedTime += (float)delta;
			//Color = new Color(Color.R, Color.G, Color.B, Mathf.Lerp(0f, 1f, elapsedTime / duration));
			Shader.SetShaderParameter("col", new Color(Color.R, Color.G, Color.B, Mathf.Lerp(0f, 1f, elapsedTime / duration)));
		}
		else
		{
			Visible = false;
			elapsedTime = 0f;
		}
	}
}