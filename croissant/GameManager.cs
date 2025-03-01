using Godot;
using GodotPlugins.Game;
using System;

public partial class GameManager : Node2D
{

    public static Window MainWindow;
    public static Window FixWindow;

    public static Vector2I ScreenSize;
    public override void _Ready()
    {
        AddFixWindow();
        ScreenSize = DisplayServer.ScreenGetSize();
    }

    public override void _Process(double delta)
    {
    }

    public void AddFixWindow()
    {
        FixWindow = new Window();
        FixWindow.Transparent = true;
        FixWindow.TransparentBg = true;
        FixWindow.AlwaysOnTop = true;
        FixWindow.Size = new Vector2I(40, 40); //min size
        FixWindow.Borderless = true;

        AddChild(FixWindow);
    }
}
