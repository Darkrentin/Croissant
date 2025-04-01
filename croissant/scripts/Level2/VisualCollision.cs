using Godot;
using System;

public partial class VisualCollision : ColorRect
{
	public float duration;
	public float elapsedTime = 0f;
	private float targetAlpha = 1f;

	public override void _Ready()
	{
		Color = new Color(1, 1, 1, 0f);
	}

	public override void _Process(double delta)
	{
		if (elapsedTime < duration)
		{
			elapsedTime += (float)delta;
			Color = new Color(1, 1, 1, Mathf.Lerp(1f - targetAlpha, targetAlpha, elapsedTime / duration));
		}
		else
		{
			Color = new Color(1, 1, 1, targetAlpha);
			Visible = false;
			elapsedTime = 0f;
		}
	}
}
