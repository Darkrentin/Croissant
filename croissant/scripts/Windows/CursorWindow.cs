using Godot;
using System;

public partial class CursorWindow : FloatWindow
{
	// Called when the node enters the scene tree for the first time.

	public CollisionShape2D collision;
	public Area2D area;
	public override void _Ready()
	{
		base._Ready();
		Size = Lib.GetScreenSize(0.1f, 0.1f);
		Size = Lib.GetAspectFactor(new Vector2I(150,150)) - TitleBarSize; 
		SetWindowPosition(Lib.GetScreenPosition(0.5f, 0.5f) - Size / 2);

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);

		if (Input.IsActionJustPressed("LeftClick"))
		{
			StartExponentialTransition(Lib.GetCursorPosition() - Size / 2, 1f, reset: true);
		}
	}

	public override void OnClose()
	{
		base.OnClose();
		StartShake(0.2f, 10);
	}

	public void TakeDamage()
	{
		IsTransitioning = false;
		if (!Shaking)
		{
			StartShake(0.2f, 10);
		}
		GameManager.MenuWindow.Open();
	}
}
