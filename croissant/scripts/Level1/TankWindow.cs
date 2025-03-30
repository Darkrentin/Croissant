using Godot;
using System;
using System.Reflection.PortableExecutable;

public partial class TankWindow : PopUpWindow
{
    [Export] public ProgressBar progressBar;
    public int clickcount = 0;
    public int maxClicks;
    public override void _Ready()
    {
        HasChangingTitle = false;
        base._Ready();
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(437, 526));
        
        maxClicks = Lib.rand.Next(1, 4);
    }

    public override void OnClose()
    {
        if (clickcount >= maxClicks)
        {
            Level1.WindowKill();
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
