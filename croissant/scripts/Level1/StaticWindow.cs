using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();

        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(400, 300));
        
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
