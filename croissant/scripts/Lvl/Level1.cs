using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Level1 : Node2D
{
    private PackedScene StaticWindowScene = ResourceLoader.Load<PackedScene>("uid://dojmcfkfdnwsu");
    public PackedScene TimerWindowScene = ResourceLoader.Load<PackedScene>("uid://ce1xhbt2knpmv");
	private PackedScene MoveWindowScene = ResourceLoader.Load<PackedScene>("uid://cb1neywi8udoc");
    private PackedScene DodgeWindowScene = ResourceLoader.Load<PackedScene>("uid://cdcpehwcb167t");
    private PackedScene TankWindowScene = ResourceLoader.Load<PackedScene>("uid://bm71aya0fw2pt");

    private PackedScene BombWindowScene = ResourceLoader.Load<PackedScene>("uid://cjcfsjb8cgs3k");
    // Called when the node enters the scene tree for the first time.
	private Timer spawnTimer;
    private Timer totalTimer;
	public int WindowKillCount{
        get => _windowKillCount;
        set {
            _windowKillCount = value;
            //GD.Print($"WindowKillCount: {_windowKillCount}");
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
        spawnTimer.WaitTime = 1.1f;
        spawnTimer.Timeout += OnSpawnTimerTimeout;
        spawnTimer.Start();

        totalTimer = new Timer();
        AddChild(totalTimer);
        totalTimer.WaitTime = 2.2f;
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
            TimerTic++;
            //GD.Print($"TimerTic increased to: {TimerTic}"); // Debug output
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
		//GD.Print($"timertic: {TimerTic}");
        GD.Print($"window: {WindowCount}");
        if (InitialWindowCount < 15)
        {
            AddNewWindow();
            InitialWindowCount++;
        }
    }


	public void AddNewWindow()
	{
        int i = Lib.rand.Next(1,TimerTic);
        /*
        if (i >= 0 &&  i <= 100)
		{
            BombWindow window = BombWindowScene.Instantiate<BombWindow>();
			AddChild(window);
		}*/
		if (i == 1)
		{
            StaticWindow window = StaticWindowScene.Instantiate<StaticWindow>();
			AddChild(window);
		}
		else if (i >= 2 &&  i <= 4)
		{
			MoveWindow window = MoveWindowScene.Instantiate<MoveWindow>();
			AddChild(window);
		}
		else if (i >= 5 && i <= 7)
        {
            TankWindow window = TankWindowScene.Instantiate<TankWindow>();
			AddChild(window);

        }
        else if (i >= 8 && i <= 13)
		{
            TimerWindow window = TimerWindowScene.Instantiate<TimerWindow>();
			AddChild(window);
		}
        else if (i >= 14 && i <= 20)
		{
            DodgeWindow window = DodgeWindowScene.Instantiate<DodgeWindow>();
			AddChild(window);
		}
        WindowCount++;
    }
}
