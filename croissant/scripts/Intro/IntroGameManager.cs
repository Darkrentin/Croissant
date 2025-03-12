using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	[Export] private Camera2D Camera;
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Bullet.tscn");
	Player Player;

	public override void _Ready()
	{
		//Window.World2D = Camera.GetWorld2D();

		Vector2I screenSize = DisplayServer.ScreenGetSize();
		GetWindow().Size = screenSize;
		Vector2I windowSize = GetWindow().Size;


		Player = GetNode<Player>("Player");
		Player.Position = windowSize / 2;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Bullet Bullet = BulletScene.Instantiate<Bullet>();
			Bullet.Position = Player.Position;
			Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
			AddChild(Bullet);
		}

	}
}
