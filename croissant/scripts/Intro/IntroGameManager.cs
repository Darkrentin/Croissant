using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Bullet.tscn");

	public override void _Ready()
	{
		Vector2I screenSize = DisplayServer.ScreenGetSize();
		GetWindow().Size = screenSize / 2;
		Vector2I windowSize = GetWindow().Size;
		GetWindow().Position = (screenSize - windowSize) / 2;

	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Bullet Bullet = BulletScene.Instantiate<Bullet>();
			Bullet.Position = GetNode<Player>("Player").Position;
			Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
			AddChild(Bullet);
		}

	}
}
