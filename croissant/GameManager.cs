using Godot;
using System;

public partial class GameManager : Node2D
{
    public static Vector2I ScreenSize;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        ScreenSize = DisplayServer.ScreenGetSize();
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
    }
}
