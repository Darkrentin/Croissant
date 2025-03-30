using Godot;
using System;

public partial class PopUpWindow : FloatWindow
{
    protected bool HasChangingTitle = false;
    protected Timer TitleTimer;

    public override void _Ready()
    {
        base._Ready();
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.9f), Lib.GetRandomNormal(0.05f, 0.9f)));
        if (HasChangingTitle)
        {
            TitleTimer = new Timer();
            AddChild(TitleTimer);
            TitleTimer.Timeout += ChangeTitle;
            ChangeTitle();
        }
    }

    protected virtual void ChangeTitle()
    {
        if (HasChangingTitle)
        {
            Title = Lib.GetCursedString();
            TitleTimer.WaitTime = Lib.GetRandomNormal(0f, 0.5f);
            TitleTimer.Start();
        }
    }
}