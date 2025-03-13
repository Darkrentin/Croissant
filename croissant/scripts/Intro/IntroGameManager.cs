using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	[Export] private Camera2D Camera;
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Bullet.tscn");
	Player Player;

	public override void _Ready()
	{
		Vector2I screenSize = DisplayServer.ScreenGetSize();
		GetWindow().Size = screenSize;
		Vector2I windowSize = GetWindow().Size;

		Camera = GetNode<Camera2D>("Camera");
		Player = GetNode<Player>("Player");
		Player.Position = windowSize / 2;
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("Shoot"))
		{
			Shoot();
			CameraShake(10);
		}
	}

	private void Shoot()
	{
		Bullet Bullet = BulletScene.Instantiate<Bullet>();
		Bullet.Position = Player.Position;
		Bullet.Rotation = Bullet.Position.AngleToPoint(GetGlobalMousePosition());
		AddChild(Bullet);
	}

	private void CameraShake(float intensity)
	{
		float shakeAmount = intensity;
	}
}
