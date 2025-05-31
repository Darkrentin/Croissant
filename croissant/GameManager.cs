using Godot;
using System;
using System.Collections.Generic;

public partial class GameManager : Node2D
{
    [Export] PackedScene menuScene;
    [Export] public GameState ExportState { get => _state; set => _state = value; }

    //Music
    [Export] public AudioStreamPlayer[] Musics;
    public enum Music { Idle, IntroGame, Level1, Level2, Level3, FinalLevel, FinalBoss, Scoreboard, NoMusic, }
    [Export] public AudioStreamPlayer BossExplosionSound;
    [Export] public bool DebugMode = false;
    public static Music CurrentMusic = Music.NoMusic;

    public static AudioStreamPlayer ClickSound;
    private static GameState _state = GameState.IntroGame;
    public static Node2D GameRoot;
    public static GameState State { get => _state; set { _state = value; } }
    public static MainWindow MainWindow;
    public static Window FixWindow;
    public static MenuWindow MenuWindow;
    public static Virus virus;
    public static Helper helper;
    public static SaveData SaveData;
    public static bool HaveFinishTheGameAtLeastOneTime = false;
    public static List<FloatWindow> Windows = new List<FloatWindow>();
    public static Vector2I ScreenSize => DisplayServer.ScreenGetSize();
    public static float ScreenScale => _cachedScreenScale;
    private static float _cachedScreenScale;
    public static bool ShakeAllWindows = false;
    public static Timer ShakeTimer;
    public static int ShakeIntensity = 0;
    public static double PersonalBestTime;
    private float _cleanupTimer = 0f;
    private const float CleanupInterval = 0.4f;

    public static Action SkipLevel;

    public static GameManager Instance;

    public enum GameState
    {
        // Dialogue states
        Virus,
        Helper,
        // Game state
        IntroGame,
        IntroVirus,
        Dialogue1,
        VirusTuto,
        Level1,
        BlueScreen,
        IntroHelper,
        Level2,
        Dialogue2,
        Level3,
        Dialogue3,
        FinalLevel,
        Scoreboard,
        IntroGameEndless,
        // Process state
        IntroGame_Process,
        // Buffer state
        Debug,
        IntroVirusBuffer,
        IntroHelperBuffer,
        Dialogue1Buffer,
        TutoBuffer,
        Level3Buffer,
        Void
    }

    public static bool IsRefocusingWindows = false;
    private static int RefocusIndex = 0;
    private static Timer RefocusTimer;

    public override void _Ready()
    {
        _cachedScreenScale = DisplayServer.ScreenGetDpi() / 96f;
        Lib.Print($"ScreenScale: {ScreenScale}");
        GameRoot = this;
        Instance = this;
        LoadSave();
        AddFixWindow();
        InitMainWindow();
        MenuWindow = menuScene.Instantiate<MenuWindow>();
        AddChild(MenuWindow);
        InitializeNpc();
        ShakeTimer = new Timer();
        ShakeTimer.Timeout += StopShakeAllWindows;
        AddChild(ShakeTimer);


        ClickSound = new AudioStreamPlayer();
        ClickSound.Stream = ResourceLoader.Load<AudioStream>("res://assets/sounds/others/click.mp3");
        ClickSound.Bus = "SFX";
        AddChild(ClickSound);

        var shaderLoaderScene = GD.Load<PackedScene>("res://scenes/Other/ShaderLoader.tscn");
        var shaderLoader = shaderLoaderScene.Instantiate<ShaderLoader>();
        AddChild(shaderLoader);
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
        if (Input.IsActionJustPressed("LeftClick") && State != GameState.FinalLevel)
            ClickSound.Play();

        if (DebugMode && Input.IsActionJustPressed("SkipLevel"))
        {
            if (SkipLevel != null)
                SkipLevel.Invoke();
        }

        _ProcessShake();

        ProcessRefocusWindows();

        _cleanupTimer += (float)delta;
        if (_cleanupTimer >= CleanupInterval)
        {
            CleanupWindowsList();
            _cleanupTimer = 0f;
        }

        switch (State)
        {
            // Dialogue states
            case GameState.Virus:
                States.Virus();
                break;
            case GameState.Helper:
                States.HelperInit();
                break;

            // Game state
            case GameState.IntroGame:
                States.IntroGame();
                break;
            case GameState.IntroVirus:
                States.IntroVirus();
                break;
            case GameState.Dialogue1:
                States.Dialogue1();
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
            case GameState.Dialogue2:
                States.Dialogue2();
                break;
            case GameState.Level3:
                States.level3();
                break;
            case GameState.Dialogue3:
                States.Dialogue3();
                break;
            case GameState.FinalLevel:
                States.FinalLevel();
                break;
            case GameState.Scoreboard:
                States.Scoreboard();
                break;
            case GameState.IntroGameEndless:
                States.IntroGameEndless();
                break;

            // _Process state
            case GameState.IntroGame_Process:
                States.IntroGame_Process();
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
            Borderless = true,
            Title = "FixWindow"
        };
        FixWindow.Name = "FixWindow";
        AddChild(FixWindow);
    }

    public void InitMainWindow()
    {
        // Load the FloatWindow script to the main window
        GetWindow().SetScript(ResourceLoader.Load("uid://ipb8ki64ej0u") as Script);
        MainWindow = GetWindow() as MainWindow;
        MainWindow.Title = "Main Window";
    }

    // Remove invalid windows from the list
    public static void CleanupWindowsList()
    {
        for (int i = Windows.Count - 1; i >= 0; i--)
        {
            FloatWindow window = Windows[i];
            if (!IsInstanceValid(window) || window.IsQueuedForDeletion() || !window.IsInsideTree())
            {
                Windows.RemoveAt(i);
            }
        }
    }

    private Vector2I _shakeOffset = Vector2I.Zero;
    private Vector2I _windowShakePos = Vector2I.Zero;

    public void _ProcessShake()
    {
        if (!ShakeAllWindows) return;

        _shakeOffset.X = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);
        _shakeOffset.Y = Lib.rand.Next(-ShakeIntensity, ShakeIntensity + 1);

        foreach (FloatWindow window in Windows)
        {
            if (window.IsTransitioning) continue;

            _windowShakePos.X = window.BasePosition.X + _shakeOffset.X;
            _windowShakePos.Y = window.BasePosition.Y + _shakeOffset.Y;

            window.SetWindowPosition(_windowShakePos, true);
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


    public void _PlayMusic(Music music)
    {
        if (CurrentMusic == music) return;

        if (music == Music.NoMusic)
        {
            _StopMusic();
            return;
        }

        int newMusicIndex = (int)music;
        int currentMusicIndex = (int)CurrentMusic;

        var newMusicPlayer = Musics[newMusicIndex];

        if (CurrentMusic != Music.NoMusic && currentMusicIndex < Musics.Length)
        {
            var currentMusicPlayer = Musics[currentMusicIndex];
            if (currentMusicPlayer.Playing)
            {
                var fadeOutTween = CreateTween();
                fadeOutTween.TweenProperty(currentMusicPlayer, "volume_db", -40.0f, 0.5f);
                fadeOutTween.TweenCallback(Callable.From(() =>
                {
                    currentMusicPlayer.Stop();
                    currentMusicPlayer.VolumeDb = 0.0f;
                }));
            }
        }

        CurrentMusic = music;
        newMusicPlayer.VolumeDb = -40.0f;
        newMusicPlayer.Play();

        var fadeInTween = CreateTween();
        fadeInTween.TweenProperty(newMusicPlayer, "volume_db", 0.0f, 0.5f);
    }

    public static void PlayMusic(Music music)
    {
        Instance._PlayMusic(music);
    }
    public void _StopMusic(bool instant = false)
    {
        if (CurrentMusic == Music.NoMusic) return;

        int currentMusicIndex = (int)CurrentMusic;
        if (currentMusicIndex >= Musics.Length)
        {
            CurrentMusic = Music.NoMusic;
            return;
        }

        var currentMusicPlayer = Musics[currentMusicIndex];
        if (currentMusicPlayer.Playing)
        {
            if (instant)
            {
                // Stop without fade
                currentMusicPlayer.Stop();
                currentMusicPlayer.VolumeDb = 0.0f;
            }
            else
            {
                // Stop with fade
                var fadeOutTween = CreateTween();
                fadeOutTween.TweenProperty(currentMusicPlayer, "volume_db", -40.0f, 0.5f);
                fadeOutTween.TweenCallback(Callable.From(() =>
                {
                    currentMusicPlayer.Stop();
                    currentMusicPlayer.VolumeDb = 0.0f;
                }));
            }
        }
        CurrentMusic = Music.NoMusic;
    }

    public static void StopMusic(bool instant = false)
    {
        Instance._StopMusic(instant);
    }

    private static void ProcessRefocusWindows()
    {
        if (!IsRefocusingWindows)
            return;

        // Si on a fini toutes les fenêtres
        if (RefocusIndex >= Windows.Count)
        {
            StopRefocusProcess();
            return;
        }

        // Refocus une fenêtre par frame
        if (RefocusIndex < Windows.Count)
        {
            FloatWindow window = Windows[RefocusIndex];
            if (IsInstanceValid(window) && !window.IsQueuedForDeletion() && window.IsInsideTree() && window.Visible)
            {
                window.GrabFocus();
            }

            RefocusIndex++;
        }
    }

    public static void StartRefocusAllWindows()
    {
        if (Windows.Count == 0 || IsRefocusingWindows) return;

        CleanupWindowsList();
        MainWindow.GrabWindowFocus();
        IsRefocusingWindows = true;
        RefocusIndex = 0;
    }

    private static void StopRefocusProcess()
    {
        IsRefocusingWindows = false;
        RefocusIndex = 0;

        // Give focus back to the main window or fix window
        if (IsInstanceValid(FixWindow))
            FixWindow.GrabFocus();
    }
    public static void ForceRefocusAllWindows()
    {
        StartRefocusAllWindows();
    }
}
