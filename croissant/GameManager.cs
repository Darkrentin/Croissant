using Godot;
using GodotPlugins.Game;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;

public partial class GameManager : Node2D
{
    [Export] PackedScene menuScene;
    public enum GameState
    {
        IntroAnimation,
        FirstVirusDialogue,
        IntroBuffer,
        Debug,
        Intro,
        Level1,
        Level2
    }

    public static Node2D GameRoot;
    private static GameState _state = GameState.Intro; //0 for normal state, -1 for debug state

    public static GameState State
    {
        get => _state;
        set
        {
            _state = value;
            Lib.Print($"State: {_state}");
            StateChange(_state);
        }
    }

    [Export]
    public GameState ExporteState
    {
        get => _state;
        set => _state = value;
    }

    public static MainWindow MainWindow;
    public static Window FixWindow;
    public static MenuWindow MenuWindow;
    public static Virus virus;

    public static List<FloatWindow> Windows = new List<FloatWindow>(); // list of all windows

    public static Vector2I ScreenSize { get => DisplayServer.ScreenGetSize(); }

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

        MenuWindow = menuScene.Instantiate<MenuWindow>();
        AddChild(MenuWindow);

        ShakeTimer = new Timer();
        ShakeTimer.Timeout += () => { StopShakeAllWindows(); };
        AddChild(ShakeTimer);

        Lib.Print($"ScreenSize: {ScreenSize}");

    }

    public override void _Process(double delta)
    {
        ProcessShake();
        CleanupWindowsList();

        switch (State)
        {
            case GameState.IntroAnimation:
                States.IntroAnimation();
                break;
            case GameState.FirstVirusDialogue:
                States.FirstVirusDialogue();
                break;
            case GameState.Intro:
                States.Intro();
                break;
            case GameState.Level1:
                States.Level1();
                break;
            case GameState.Level2:
                States.Level2();
                break;
            case GameState.Debug:
                //debug State
                States.StateDebug();
                break;
            default:
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
        GetWindow().SetScript(ResourceLoader.Load("uid://ipb8ki64ej0u") as Script);
        MainWindow = GetWindow() as MainWindow;
    }

    public static void CleanupWindowsList()
    {
        // Create a new list to store valid windows
        List<FloatWindow> validWindows = new List<FloatWindow>();
        List<FloatWindow> invalidWindows = new List<FloatWindow>();

        // Check each window in the list
        foreach (FloatWindow window in Windows)
        {
            // Skip null references
            if (window == null)
            {
                invalidWindows.Add(window);
                continue;
            }

            // Safely check if the window is valid
            try
            {
                // Try to access a property that won't change the state
                // but will throw if the object is disposed
                bool isValid = window.IsInsideTree() && !window.IsQueuedForDeletion();
                if (isValid)
                {
                    validWindows.Add(window);
                }
                else
                {
                    invalidWindows.Add(window);
                }
            }
            catch (ObjectDisposedException)
            {
                // Window is already disposed, add it to invalid windows
                invalidWindows.Add(window);
            }
            catch (Exception e)
            {
                // Some other exception occurred
                GD.PushWarning($"Exception when checking window validity: {e.Message}");
                invalidWindows.Add(window);
            }
        }

        // Replace the Windows list with only the valid windows
        if (invalidWindows.Count > 0)
        {
            Lib.Print($"CleanupWindowsList: {invalidWindows.Count} windows removed");
            Windows = validWindows;
        }
    }

    public void ProcessShake()
    {
        if (ShakeAllWindows)
        {
            int offsetX = (int)Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
            int offsetY = (int)Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);

            foreach (FloatWindow w in Windows)
            {
                Vector2I ShakePosition = w.BasePosition + new Vector2I(offsetX, offsetY);
                w.SetWindowPosition(ShakePosition);
            }
        }
    }

    public static void StartShakeAllWindows(float duration, int intensity)
    {
        foreach (FloatWindow w in Windows)
        {
            w.BasePosition = w.Position;
        }
        ShakeAllWindows = true;
        ShakeIntensity = intensity;
        if (duration != 0)
        {
            ShakeTimer.Start(duration);
        }
    }

    public static void StopShakeAllWindows()
    {
        ShakeAllWindows = false;
        foreach (FloatWindow w in Windows)
        {
            w.Position = w.BasePosition;
        }
    }

    public static void StateChange(GameState state)
    {
    }
}
