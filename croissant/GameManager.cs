using Godot;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public enum DifficultyLevel
{
    Easy,
    Medium,
    Hard
}

public partial class GameManager : Node2D
{
    [Export] public float difficulty {get =>_difficulty; set => _difficulty = value; }
    private static float _difficulty = 1f;
    public static float Difficulty
    {
        get => _difficulty;
        private set => _difficulty = value;
    }

    public static void SetDifficulty(DifficultyLevel level)
    {
        switch (level)
        {
            case DifficultyLevel.Easy:
                Difficulty = 0.75f;
                break;
            case DifficultyLevel.Medium:
                Difficulty = 1f;
                break;
            case DifficultyLevel.Hard:
                Difficulty = 1.5f;
                break;
        }
    }

    [Export] PackedScene menuScene;
    public enum GameState
    {
        Virus,
        // Game state
        Difficulty,
        IntroGame,
        IntroVirus,
        VirusDialogue1,
        VirusTuto,
        Level1,
        Level2,
        // Process state
        IntroGameProcess,
        // Buffer state
        Debug,
        IntroVirusBuffer,
        VirusDialogue1Buffer,
        TutoBuffer,
        Void
    }
    public static Node2D GameRoot;
    private static GameState _state = GameState.Difficulty; // 0 : normal, -1 : debug
    public static GameState State
    {
        get => _state;
        set
        {
            _state = value;
            //Lib.Print($"State: {_state}");
            StateChange(_state);
        }
    }
    [Export]
    public GameState ExportState
    {
        get => _state;
        set => _state = value;
    }
    public static MainWindow MainWindow;
    public static Window FixWindow;
    public static MenuWindow MenuWindow;
    public static Virus virus;
    public static List<FloatWindow> Windows = new List<FloatWindow>();
    public static Vector2I ScreenSize => DisplayServer.ScreenGetSize();
    public static bool ShakeAllWindows = false;
    public static Timer ShakeTimer;
    public static int ShakeIntensity = 0;


    public override void _Ready()
    {
        GameRoot = this;
        AddFixWindow();
        InitMainWindow();
        // Le reste du code _Ready existant
        MenuWindow = menuScene.Instantiate<MenuWindow>();
        AddChild(MenuWindow);

        ShakeTimer = new Timer();
        ShakeTimer.Timeout += StopShakeAllWindows;
        AddChild(ShakeTimer);
    }

    private void InitializeGame()
    {
        // Initialisation du jeu après la sélection de la difficulté
        State = GameState.IntroGame;
    }

    public override void _Process(double delta)
    {
        ProcessShake();
        CleanupWindowsList();

        switch (State)
        {
            case GameState.Virus:
                States.Virus();
                break;

            // Game state
            case GameState.Difficulty:
                States.ChooseDifficulty();
                break;
            case GameState.IntroGame:
                States.IntroGame();
                break;
            case GameState.IntroVirus:
                States.IntroVirus();
                break;
            case GameState.VirusDialogue1:
                States.VirusDialogue1();
                break;
            case GameState.VirusTuto:
                States.VirusTuto();
                break;
            case GameState.Level1:
                States.Level1();
                break;
            case GameState.Level2:
                States.Level2();
                break;

            // Process state
            case GameState.IntroGameProcess:
                States.IntroGameProcess(delta);
                break;

            // Buffer state
            case GameState.Debug:
                States.Debug();
                break;

            default:
                break;
        }
    }

    // Create FixWindow, to get the focus, the right mouse position, and particles
    public void AddFixWindow()
    {
        FixWindow = new Window
        {
            Transparent = true,
            TransparentBg = true,
            AlwaysOnTop = true,
            Size = new Vector2I(40, 40),
            Borderless = true
        };
        AddChild(FixWindow);
    }

    public void InitMainWindow()
    {
        // Load the FloatWindow script to the main window
        GetWindow().SetScript(ResourceLoader.Load("uid://ipb8ki64ej0u") as Script);
        MainWindow = GetWindow() as MainWindow;
    }

    // Remove invalid windows from the list
    public static void CleanupWindowsList()
    {
        List<FloatWindow> validWindows = new List<FloatWindow>();
        foreach (FloatWindow window in Windows)
        {
            if (window == null)
                continue;
            try
            {
                if (window.IsInsideTree() && !window.IsQueuedForDeletion())
                    validWindows.Add(window);
            }
            catch (ObjectDisposedException)
            {
                //Lib.Print("A window was already disposed and removed from the list.");
            }
            catch (Exception e)
            {
                GD.PushWarning($"Unexpected exception when checking window validity: {e.Message}");
            }
        }
        Windows = validWindows;
    }

    public void ProcessShake()
    {
        if (!ShakeAllWindows)
            return;

        int offsetX = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
        int offsetY = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
        Vector2I offset = new Vector2I(offsetX, offsetY);

        foreach (FloatWindow window in Windows)
        {
            if (window.IsTransitioning)
                continue;
            Vector2I shakePosition = window.BasePosition + offset;
            window.SetWindowPosition(shakePosition);
        }
    }

    public static void StartShakeAllWindows(float duration, int intensity)
    {
        foreach (FloatWindow window in Windows)
            window.BasePosition = window.Position;

        ShakeAllWindows = true;
        ShakeIntensity = intensity;

        if (duration > 0)
            ShakeTimer.Start(duration);
    }

    public static void StopShakeAllWindows()
    {
        ShakeAllWindows = false;

        foreach (FloatWindow window in Windows)
            window.Position = window.BasePosition;
    }

    public static void StateChange(GameState state) { }
}
