using Godot;
using System;

public partial class TankWindow : PopUpWindow
{
    [Export] public ProgressBar progressBar;
    public int clicks = 0;
    public int HPs = 3;
    public override void _Ready()
    {
        HasChangingTitle = false;
        base._Ready();
        Size = Lib.GetAspectFactor(new Vector2I(437, 526));
        Title = "❤︎❤︎❤︎";
    }

    public override void OnClose()
    {
        if (clicks == 0)
            Title = "❤︎❤︎";
        else if (clicks == 1)
            Title = "❤︎";
        else
        {
            Level1.WindowKill();
            QueueFree();
        }
        clicks++;
        //Lib.Print($"Clickcount: {clickcount}");
        StartShake(0.15f, 10);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
