using Godot;
using GodotPlugins.Game;
using System;

public partial class GameManager : Node2D
{
    public static FloatWindow MainWindow;
    public static Window FixWindow;
    
    public static Vector2I ScreenSize;

    public override void _Ready()
    {
        AddFixWindow();
        ScreenSize = DisplayServer.ScreenGetSize();
        GetWindow().SetScript(ResourceLoader.Load("res://scripts/FloatWindow.cs") as Script);
        MainWindow = GetWindow() as FloatWindow;
        MainWindow.Draggable = false;
        MainWindow.transitionMode = FloatWindow.TransitionMode.Exponential;
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
    
    // Convertit des coordonnées relatives (0.0-1.0) en position d'écran absolue
    public static Vector2I GetScreenPosition(float relativeX, float relativeY)
    {
        return new Vector2I(
            (int)(ScreenSize.X * relativeX),
            (int)(ScreenSize.Y * relativeY)
        );
    }

    // Convertit une taille relative (0.0-1.0) en taille d'écran absolue
    public static Vector2I GetScreenSize(float relativeWidth, float relativeHeight)
    {
        return new Vector2I(
            (int)(ScreenSize.X * relativeWidth),
            (int)(ScreenSize.Y * relativeHeight)
        );
    }
}
