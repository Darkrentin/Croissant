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
    public int WindowKillCount
    {
        get => _windowKillCount;
        set
        {
            _windowKillCount = value;
            //Lib.Print($"WindowKillCount: {_windowKillCount}");
        }
    }
    private int _windowKillCount = 0;
    public int WindowCount;
    public int InitialWindowCount = 0;
    public int TimerTic = 2;
    public List<FloatWindow> Windows = new List<FloatWindow>();

    public override void _Ready()
    {
        WindowKillCount = 0;
        WindowCount = 0;

        spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = 1f;
        spawnTimer.Timeout += OnSpawnTimerTimeout;
        spawnTimer.Start();

        totalTimer = new Timer();
        AddChild(totalTimer);
        totalTimer.WaitTime = 1.8f;
        totalTimer.Timeout += TotalSpawnerTimeout;
        totalTimer.Start();

    }

    public void OnSpawnTimerTimeout()
    {
        if (WindowCount < 20 && WindowCount > 0)
        {
            AddNewWindow();
            //UpdateSpawnTimer();
        }
    }


    public void TotalSpawnerTimeout()
    {
        if (TimerTic < 21)
        {
            if (_windowKillCount >= TimerTic * 1.5)
            {
                TimerTic++;
            }
            //Lib.Print($"TimerTic increased to: {TimerTic}"); // Debug output
        }
        else
        {
            spawnTimer.WaitTime = 5f;
        }
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
        if (InitialWindowCount < 15)
        {
            AddNewWindow();
            InitialWindowCount++;
        }
    }


    public void AddNewWindow()
    {
        int i = Lib.rand.Next(1, TimerTic);
        if (i == 1)
        {
            StaticWindow window = StaticWindowScene.Instantiate<StaticWindow>();
            AddChild(window);
        }
        else if (i >= 2 && i <= 5)
        {
            MoveWindow window = MoveWindowScene.Instantiate<MoveWindow>();
            AddChild(window);
        }
        else if (i >= 6 && i <= 8)
        {
            BombWindow window = BombWindowScene.Instantiate<BombWindow>();
            AddChild(window);
        }
        else if (i >= 9 && i <= 12)
        {
            TankWindow window = TankWindowScene.Instantiate<TankWindow>();
            AddChild(window);
        }
        else if (i >= 13 && i <= 16)
        {
            TimerWindow window = TimerWindowScene.Instantiate<TimerWindow>();
            AddChild(window);
        }
        else if (i >= 17 && i <= 20)
        {
            DodgeWindow window = DodgeWindowScene.Instantiate<DodgeWindow>();
            AddChild(window);
        }
        WindowCount++;
    }
}
