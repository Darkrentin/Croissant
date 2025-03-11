using Godot;
using GodotPlugins.Game;
using System;

public partial class GameManager : Node2D
{

    public static FloatWindow MainWindow;
    public static Window FixWindow;
    
    public static Vector2I ScreenSize;

    public static Vector2 SizeRatio;
    public override void _Ready()
    {
        AddFixWindow();
        ScreenSize = DisplayServer.ScreenGetSize();
        GetWindow().SetScript(ResourceLoader.Load("res://scripts/FloatWindow.cs") as Script);
        MainWindow = GetWindow() as FloatWindow;
        MainWindow.Draggable = false;
        MainWindow.transitionMode = FloatWindow.TransitionMode.Exponential;
        
        Vector2 BaseResolution = new Vector2(1920, 1080);
        SizeRatio = ScreenSize/ BaseResolution;
        GD.Print("ScreenRatio: " + SizeRatio);

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
