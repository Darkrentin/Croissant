using Godot;
using System;
using System.Reflection.PortableExecutable;

public partial class TankWindow : PopUpWindow
{
    [Export] public ProgressBar progressBar;
    private int clickcount = 0;
    private int maxClicks;
    public override void _Ready()
    {
        base._Ready();
        Parent = GetParent<Level1>();
        Size = Lib.GetScreenSize(Lib.GetPercentage(new Vector2I(437, 526)));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
        maxClicks = Lib.rand.Next(1, 4);
    }

    public override void OnClose()
    {
        if (clickcount >= maxClicks)
        {
            Parent.WindowKillCount++;
            Parent.WindowCount--;
            QueueFree();
        }
        else
        {
            clickcount++;
        }
        //Lib.Print($"Clickcount: {clickcount}");
        StartShake(0.2f, 10);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Title = "";
        for (int i = 0; i <= maxClicks - clickcount; i++)
        {
            Title += "▯";
        }
    }
}
