using Godot;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

public partial class Level1 : Node2D
{
    private PackedScene StaticWindowScene = ResourceLoader.Load<PackedScene>("res://scenes/StaticWindow.tscn");
    public PackedScene TimerWindowScene = ResourceLoader.Load<PackedScene>("res://scenes/TimerWindow.tscn");
	private PackedScene MoveWindowScene = ResourceLoader.Load<PackedScene>("res://scenes/MoveWindow.tscn");
    // Called when the node enters the scene tree for the first time.
	private Timer spawnTimer;
	public int WindowKillCount{
        get => _windowKillCount;
        set {
            _windowKillCount = value;
            GD.Print($"WindowKillCount: {_windowKillCount}");
        }
    }
    private int _windowKillCount = 0;

	public int WindowCount;
	public List<FloatWindow> Windows = new List<FloatWindow>();

    public override void _Ready()
    {
        WindowKillCount = 0;
        WindowCount = 0;
		spawnTimer = new Timer();
        AddChild(spawnTimer);
        spawnTimer.WaitTime = 1.5f;
        spawnTimer.Timeout += OnSpawnTimerTimeout;
        spawnTimer.Start();
        
        for (int i = 0; i < 5; i++)
        {
			AddNewWindow();
        }
		
    }

	private void OnSpawnTimerTimeout()
    {
        if (WindowCount < 12)
        {
            AddNewWindow();		
        }
    }


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
		/*if (WindowCount<15)
		{
			AddNewWindow();
			WindowCount++;
		}*/
		GD.Print($"WindowCount: {WindowCount}");
    }


	public void AddNewWindow()
	{
		int i = Lib.rand.Next(1,4);
			if (i%3 == 0)
			{
				MoveWindow window = MoveWindowScene.Instantiate<MoveWindow>();
				AddChild(window);
			}
			if (i%2 == 0)
			{
				TimerWindow window = TimerWindowScene.Instantiate<TimerWindow>();
				AddChild(window);
			}
			else
			{
		    	StaticWindow window = StaticWindowScene.Instantiate<StaticWindow>();
				AddChild(window);
			}
        WindowCount++;
    }
}
