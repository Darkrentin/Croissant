using Godot;
using System.Collections.Generic;

public partial class Level1 : Node2D
{
    [Export] private AudioStreamPlayer PopupEnter;
    [Export] private AudioStreamPlayer PopupClose;
    [Export] private PackedScene StaticWindowScene;
    [Export] private PackedScene TimerWindowScene;
    [Export] private PackedScene MoveWindowScene;
    [Export] private PackedScene DodgeWindowScene;
    [Export] private PackedScene TankWindowScene;
    [Export] private PackedScene BombWindowScene;

    public float StaticMultiplier = 0.7f;
    public float TimerWindowMultiplier = 1.5f;
    public float MoveWindowMultiplier = 1.5f;
    public float DodgeWindowMultiplier = 4.1f;
    public float TankWindowMultiplier = 3.5f;
    public float BombWindowMultiplier = 2.0f;
    public float TimerBasicTime = 0.41f;
    public float TimerMultiplier = 1.0f;
    private Timer spawnTimer;
    public static int WindowCount;
    public static int InitialWindowCount = 0;
    public static Level1 Instance;
    public List<FloatWindow> Windows = new List<FloatWindow>();
    public override void _Ready()
    {
        Instance = this;
        WindowCount = 0;

        spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = TimerBasicTime;
        spawnTimer.Timeout += AddNewWindow;
        spawnTimer.Start();
    }

    public override void _Process(double delta)
    {
        if (InitialWindowCount < 22) //game start
        {
            AddStaticWWindow();
            InitialWindowCount++;
        }
        if (WindowCount == 0) //end game condition
        {
            Lib.Print("No windows left, ending game...");
            GameManager.State = GameManager.GameState.BlueScreen;
            QueueFree();
        }

    }

    public static void AddStaticWWindow()
    {
        StaticWindow window = Instance.StaticWindowScene.Instantiate<StaticWindow>();
        Instance.AddChild(window);
        Instance.Windows.Add(window);
        WindowCount++;
        Lib.Print($"WindowAdd : count: {WindowCount}");
    }

    public static void AddNewWindow()
    {
        if (WindowCount < 22 && WindowCount > 0 && InitialWindowCount >= 22)
        {
            var scenes = new PackedScene[]
            {
                Instance.StaticWindowScene,
                Instance.MoveWindowScene,
                Instance.BombWindowScene,
                Instance.TankWindowScene,
                Instance.TimerWindowScene,
                Instance.DodgeWindowScene
            };

            var multipliers = new float[]
            {
                Instance.StaticMultiplier,
                Instance.MoveWindowMultiplier,
                Instance.BombWindowMultiplier,
                Instance.TankWindowMultiplier,
                Instance.TimerWindowMultiplier,
                Instance.DodgeWindowMultiplier
            };

            int i = Lib.rand.Next(0, scenes.Length);
            FloatWindow window = scenes[i].Instantiate<FloatWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
            Instance.PopupEnter.Play();

            Instance.spawnTimer.Stop();
            Instance.spawnTimer.WaitTime = Instance.TimerBasicTime * multipliers[i] * Instance.TimerMultiplier;
            Instance.spawnTimer.Start();

            WindowCount++;
            Instance.TimerMultiplier += 0.033f;

            //Lib.Print($"WindowAdd : count: {WindowCount}");

            Lib.Print($"TimerMultiplier: {Instance.TimerMultiplier}");
            Lib.Print($"WaitTime: {Instance.spawnTimer.WaitTime}");
        }
    }



    public static void WindowKill()
    {
        Instance.PopupClose.Play();
        WindowCount--;
        //Lib.Print($"WindowKill : count: {WindowCount}");
    }
}
