using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 300));
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
    }

    public override void OnClose()
    {
        Level1.WindowKill();
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
