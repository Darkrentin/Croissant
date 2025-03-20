using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
    private Timer Timer = new Timer();
    public override void _Ready()
    {
        base._Ready();

        AddChild(Timer);
        Timer.Timeout += ChangeTitle;
        ChangeTitle();

        Parent = GetParent<Level1>();
        Size = Lib.GetScreenSize(Lib.GetPercentage(new Vector2I(400, 300)));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
    }

    public override void OnClose()
    {
        Parent.WindowKillCount++;
        Parent.WindowCount--;
        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void ChangeTitle()
    {
        Title = Lib.GetCursedString();
        Timer.WaitTime = Lib.GetRandomNormal(0, 2);
        Timer.Start();
    }

}
