using Godot;
using System;

public partial class VisualCollision : ColorRect
{
	// Called when the node enters the scene tree for the first time.

	private float elapsedTime = 0f;
	private float duration = 0.2f;
	private float targetAlpha = 0.2f;
	public override void _Ready()
	{
		Color = new Color(1, 1, 1, 1f);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		elapsedTime += (float)delta;
		if (elapsedTime >= duration)
		{
			elapsedTime = 0f;
			Color = new Color(1, 1, 1, targetAlpha);
			targetAlpha = 1f - targetAlpha;
			/*
			duration-=0.01f;
			if (duration <= 0.1f)
			{
				duration = 0.5f;
			}
			*/
		}
		else
		{
			Color = new Color(1, 1, 1, Mathf.Lerp(1f - targetAlpha, targetAlpha, elapsedTime / duration));
		}
	}
}
