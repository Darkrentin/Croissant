using Godot;
using System;

public partial class IntroGameManager : Node2D
{
	PackedScene BulletScene = ResourceLoader.Load<PackedScene>("res://scenes/intro_bullet.tscn");
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetWindow().Size = DisplayServer.ScreenGetSize() / 2;
		GetWindow().InitialPosition = Window.WindowInitialPosition.CenterMainWindowScreen;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("shoot"))
		{
			IntroBullet bullet = BulletScene.Instantiate<IntroBullet>();
			bullet.Position = GetNode<IntroPlayer>("IntroPlayer").Position;
			bullet.Rotation = bullet.Position.AngleToPoint(GetGlobalMousePosition());
			AddChild(bullet);
		}

	}
}
