using Godot;
using System;
using System.Collections.Generic;

public partial class Level1 : Node2D
{
    [Export] private PackedScene StaticWindowScene;
    [Export] private PackedScene TimerWindowScene;
    [Export] private PackedScene MoveWindowScene;
    [Export] private PackedScene DodgeWindowScene;
    [Export] private PackedScene TankWindowScene;
    [Export] private PackedScene BombWindowScene;
    // Called when the node enters the scene tree for the first time.
    private Timer spawnTimer;
    private Timer totalTimer;
    public static int WindowKillCount
    {
        get => _windowKillCount;
        set
        {
            _windowKillCount = value;
            //Lib.Print($"WindowKillCount: {_windowKillCount}");
        }
    }
    private static int _windowKillCount = 0;
    public static int WindowCount;
    public int InitialWindowCount = 0;
    public float TimerLimit = 0.025f;
    public int TimerTic = 2;

    public static Level1 Instance;

    public List<FloatWindow> Windows = new List<FloatWindow>();

    public override void _Ready()
    {
        Instance = this;
        WindowKillCount = 0;
        WindowCount = 0;

        spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = 1f;
        spawnTimer.Timeout += OnSpawnTimerTimeout;
        spawnTimer.Start();

        totalTimer = new Timer();
        AddChild(totalTimer);
        totalTimer.WaitTime = 10f;
        totalTimer.Timeout += TotalSpawnerTimeout;
        totalTimer.Start();

    }

    public void OnSpawnTimerTimeout()
    {
        if (WindowCount < 20 && WindowCount > 0)
        {
            AddNewWindow();
            spawnTimer.WaitTime = Lib.rand.NextDouble() * 0.6f + 0.4f + TimerLimit;
            //UpdateSpawnTimer();
        }
    }


    public void TotalSpawnerTimeout()
    {
        if (TimerTic < 20)
        {
            TimerTic++;
        }
        else
        {
            //spawnTimer.WaitTime = 5f;
        }
        TimerLimit *= 1.15f;
    }

    /*private void UpdateSpawnTimer()
    {
        if (WindowCount >= 12)
        {
            spawnTimer.WaitTime = 3f;
        }
        else if (WindowCount >= 6)
        {
            spawnTimer.WaitTime = 1.5f;
        }
        else
        {
            spawnTimer.WaitTime = 1.3f;
        }
    }*/

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        /*if (WindowCount<15)
		{
			AddNewWindow();
			WindowCount++;
		}*/
        //Lib.Print($"timertic: {TimerTic}");
        //Lib.Print($"window: {WindowCount}");
        if (InitialWindowCount < 22)
        {
            AddNewWindow();
            InitialWindowCount++;
        }
        else if (InitialWindowCount == 22)
        {
            totalTimer.WaitTime = 1.5f;
            InitialWindowCount++;
        }
    }


    public static void AddNewWindow()
    {
        int i = Lib.rand.Next(1, Instance.TimerTic);
        if (i == 1)
        {
            StaticWindow window = Instance.StaticWindowScene.Instantiate<StaticWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        else if (i >= 2 && i <= 5)
        {
            MoveWindow window = Instance.MoveWindowScene.Instantiate<MoveWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        else if (i >= 6 && i <= 7)
        {
            BombWindow window = Instance.BombWindowScene.Instantiate<BombWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        else if (i >= 8 && i <= 11)
        {
            TankWindow window = Instance.TankWindowScene.Instantiate<TankWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        else if (i >= 12 && i <= 15)
        {
            TimerWindow window = Instance.TimerWindowScene.Instantiate<TimerWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        else if (i >= 16 && i <= 19)
        {
            DodgeWindow window = Instance.DodgeWindowScene.Instantiate<DodgeWindow>();
            Instance.AddChild(window);
            Instance.Windows.Add(window);
        }
        WindowCount++;
    }

    public static void WindowKill()
    {
        WindowKillCount++;
        WindowCount--;
    }
}
