using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    [Export] PackedScene menuScene;
    [Export] public GameState ExportState { get => _state; set => _state = value; }
    private static GameState _state = GameState.IntroGame;
    public static Node2D GameRoot;
    public static GameState State { get => _state; set { _state = value; StateChange(_state); } }
    public static MainWindow MainWindow;
    public static Window FixWindow;
    public static MenuWindow MenuWindow;
    public static Virus virus;
    public static Helper helper;
    public static SaveData SaveData;
    public static bool HaveFinishTheGameAtLeastOneTime = false;
    public static List<FloatWindow> Windows = new List<FloatWindow>();
    public static Vector2I ScreenSize => DisplayServer.ScreenGetSize();
    public static float ScreenScale => DisplayServer.ScreenGetDpi()/96f; // 96 DPI is the default for most monitors
    public static bool ShakeAllWindows = false;
    public static Timer ShakeTimer;
    public static int ShakeIntensity = 0;
    public static double PersonalBestTime;
    public enum GameState
    {
        Virus,
        Helper,
        Scoreboard,
        // Game state
        IntroGame,
        IntroVirus,
        VirusDialogue1,
        VirusTuto,
        Level1,
        BlueScreen,
        IntroHelper,
        Level2,
        HelperDialogue1,
        Level3,
        FinalLevel,
        // _Process state
        IntroGame_Process,
        // Buffer state
        Debug,
        IntroVirusBuffer,
        IntroHelperBuffer,
        VirusDialogue1Buffer,
        TutoBuffer,
        Void
    }

    public override void _Ready()
    {
        Lib.Print($"ScreenScale: {ScreenScale}");
        GameRoot = this;
        LoadSave();
        AddFixWindow();
        InitMainWindow();
        // Le reste du code _Ready existant
        MenuWindow = menuScene.Instantiate<MenuWindow>();
        AddChild(MenuWindow);
        InitializeNpc();
        ShakeTimer = new Timer();
        ShakeTimer.Timeout += StopShakeAllWindows;
        AddChild(ShakeTimer);
    }

    private void InitializeNpc()
    {
        virus = States.VirusScene.Instantiate<Virus>();
        GameRoot.AddChild(virus);
        virus.Position = Lib.GetScreenPosition(-0.5f, -0.5f);
        virus.ForceDialoguePlacement = true;
        virus.On = false;

        helper = States.HelperScene.Instantiate<Helper>();
        GameRoot.AddChild(helper);
        helper.Position = Lib.GetScreenPosition(-0.5f, -0.5f);
        helper.ForceDialoguePlacement = true;

        virus.Visible = false;
        helper.Visible = false;
    }

    public void LoadSave()
    {
        SaveData = SaveData.LoadData();
        if (SaveData == null)
        {
            SaveData = new SaveData();
            //SaveData.Save();
            Lib.Print("SaveData is null, creating a new one.");
        }
        else
        {
            HaveFinishTheGameAtLeastOneTime = SaveData.HaveFinishTheGameAtLeastOneTime;
            PersonalBestTime = SaveData.PersonalBestTime;
        }
    }

    private void InitializeGame()
    {
        State = GameState.IntroGame;
    }

    public override void _Process(double delta)
    {
        _ProcessShake();
        CleanupWindowsList();

        switch (State)
        {
            case GameState.Virus:
                States.Virus();
                break;
            case GameState.Helper:
                States.Helper();
                break;
            case GameState.Scoreboard:
                States.Scoreboard();
                break;

            // Game state
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
                States.VirusTutoSelection();
                break;
            case GameState.Level1:
                States.Level1();
                break;
            case GameState.BlueScreen:
                States.BlueScreen();
                break;
            case GameState.IntroHelper:
                States.IntroHelper();
                break;
            case GameState.Level2:
                States.Level2();
                break;
            case GameState.HelperDialogue1:
                States.HelperDialogue1();
                break;
            case GameState.Level3:
                States.Level3();
                break;
            case GameState.FinalLevel:
                States.FinalLevel();
                break;

            // _Process state
            case GameState.IntroGame_Process:
                States.IntroGame_Process(delta);
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
            Size = new Vector2I(1, 1),
            //Visible = false,
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
                ////Lib.Print("A window was already disposed and removed from the list.");
            }
            catch (Exception e)
            {
                GD.PushWarning($"Unexpected exception when checking window validity: {e.Message}");
            }
        }
        Windows = validWindows;
    }

    public void _ProcessShake()
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
