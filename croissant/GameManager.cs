using Godot;
using System;

public partial class GameManager : Node2D
{
    public static Vector2I ScreenSize;

    public static Window MainWindow;

    public override void _Ready()
    {
        ScreenSize = DisplayServer.ScreenGetSize();
        MainWindow = GetWindow();
    }

    public override void _Process(double delta)
    {
        
    }
}
