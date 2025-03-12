using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.Dynamic;

public partial class GameManager : Node2D
{
    public static FloatWindow MainWindow;
    public static Window FixWindow;

    public static List<Window> Windows = new List<Window>(); // list of all windows
    
    public static Vector2I ScreenSize {get => DisplayServer.ScreenGetSize();}

    public override void _Ready()
    {
        AddFixWindow();
        InitMainWindow();
        GD.Print($"ScreenSize: {ScreenSize}");

    }

    public override void _Process(double delta)
    {
    }

    //Create the FixWindow
    //This window is used to fix the focus, get the right mouse position and other things
    public void AddFixWindow()
    {
        //set the FixWindow properties to be transparent and borderless
        FixWindow = new Window();
        FixWindow.Transparent = true;
        FixWindow.TransparentBg = true;
        FixWindow.AlwaysOnTop = true;
        FixWindow.Size = new Vector2I(40, 40); //min size
        FixWindow.Borderless = true;

        AddChild(FixWindow);
    }

    //because we can't acces the main window before the execution of the _Ready function
    //we need to call this function to initialize the main window
    //this allow us to use the main window like any other window
    public void InitMainWindow()
    {
        //Load the FloatWindow script to the main window
        GetWindow().SetScript(ResourceLoader.Load("res://scripts/MainWindow.cs") as Script);
        MainWindow = GetWindow() as FloatWindow;
    }
    
    //Get a Position on the screen based on a relative position
    //relativeX and relativeY are values between 0.0 and 1.0
    //this function allows you to work with relative positions and not absolute positions to make the game resolution independent
    public static Vector2I GetScreenPosition(float relativeX, float relativeY)
    {
        return new Vector2I(
            (int)(ScreenSize.X * relativeX),
            (int)(ScreenSize.Y * relativeY)
        );
    }

    //Get a Size on the screen based on a relative size
    //relativeWidth and relativeHeight are values between 0.0 and 1.0
    //this function allows you to work with relative sizes and not absolute sizes to make the game resolution independent
    public static Vector2I GetScreenSize(float relativeWidth, float relativeHeight)
    {
        return new Vector2I(
            (int)(ScreenSize.X * relativeWidth),
            (int)(ScreenSize.Y * relativeHeight)
        );
    }
}
