using Godot;
using System;

public partial class TestScene : Node2D
{
    [Export] Window window;
    [Export] Camera2D cam;

    public Vector2I LastPos = Vector2I.Zero;
    public Vector2I velocity = Vector2I.Zero;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        cam.AnchorMode = Camera2D.AnchorModeEnum.FixedTopLeft;
        window.Transient = true;
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here
        velocity = window.Position - LastPos;
        LastPos = window.Position;
        cam.Position = window.Position +velocity ;
    }
}
