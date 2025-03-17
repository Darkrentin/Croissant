using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;

public partial class GameManager : Node2D
{

    public static Node2D GameRoot;
    private static int _state = -1; //0 for normal state, -1 for debug state

    public static int State
    {
        get => _state;
        set
        {
            _state = value;
            GD.Print($"State: {_state}");
            StateChange(_state);
        }
    }

    [Export] public int ExporteState
    {
        get => _state;
        set => _state = value;
    }

    public static MainWindow MainWindow;
    public static Window FixWindow;
    public static MenuWindow MenuWindow;

    public static List<FloatWindow> Windows = new List<FloatWindow>(); // list of all windows
    
    public static Vector2I ScreenSize {get => DisplayServer.ScreenGetSize();}

    public static bool ShakeAllWindows = false;
    public static Timer ShakeTimer;
    public static int ShakeIntensity = 0;

    public override void _Ready()
    {
        GameRoot = this;
        AddFixWindow();
        InitMainWindow();
        //Windows.Add(MainWindow);
        //Windows.Add(FixWindow);

        PackedScene menuScene = ResourceLoader.Load("uid://810np22fqce") as PackedScene;
        MenuWindow = menuScene.Instantiate<MenuWindow>();
        AddChild(MenuWindow);

        ShakeTimer = new Timer();
        ShakeTimer.Timeout+=()=>{StopShakeAllWindows();};
        AddChild(ShakeTimer);

        GD.Print($"ScreenSize: {ScreenSize}");

    }

    public override void _Process(double delta)
    {
        ProcessShake();

        switch(State)
        {
            case 0:
                State0.Process();
                break;
            case -1:
                //debug State
                break;
            default:
                GD.PushError("Invalid State");
                break;
        }
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
        MainWindow = GetWindow() as MainWindow;
    }

    public void ProcessShake()
    {
        if(ShakeAllWindows)
        {
			int offsetX = (int)Lib.rand.Next(-ShakeIntensity,ShakeIntensity+1);
			int offsetY = (int)Lib.rand.Next(-ShakeIntensity,ShakeIntensity+1);

            foreach(FloatWindow w in Windows)
            {
                Vector2I ShakePosition = w.BasePosition + new Vector2I(offsetX,offsetY);
                w.SetWindowPosition(ShakePosition);
            }
        }
    }

    public static void StartShakeAllWindows(float duration,int intensity)
    {
        foreach(FloatWindow w in Windows)
        {
            w.BasePosition = w.Position;
        }
        ShakeAllWindows = true;
        ShakeIntensity = intensity;
        if(duration!=0)
        {
            ShakeTimer.Start(duration);
        }
    }

    public static void StopShakeAllWindows()
    {
        ShakeAllWindows = false;
        foreach(FloatWindow w in Windows)
        {
            w.Position = w.BasePosition;
        }
    }

    public static void StateChange(int state)
    {
    }
}
