using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
    private Timer TitleTimer = new Timer();
    public override void _Ready()
    {
        base._Ready();

        AddChild(TitleTimer);
        TitleTimer.Timeout += ChangeTitle;
        ChangeTitle();

        Parent = GetParent<Level1>();
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 300));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
    }

    public override void OnClose()
    {
        Parent.WindowKillCount++;
        Parent.WindowCount--;
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void ChangeTitle()
    {
        Title = Lib.GetCursedString();
        TitleTimer.WaitTime = Lib.GetRandomNormal(0f, 0.5f);
        TitleTimer.Start();
    }
}
