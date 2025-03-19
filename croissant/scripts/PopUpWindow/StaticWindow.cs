using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Parent = GetParent<Level1>();
        Size = new Vector2I(400, 200);
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.90f), Lib.GetRandomNormal(0f, 0.90f)));
    }

    public override void OnClose()
    {
        // Make sure this window is removed from GameManager.Windows list
        if (GameManager.Windows.Contains(this))
        {
            GameManager.Windows.Remove(this);
        }

        // Also remove from any window's CollisionWindows list
        foreach (FloatWindow window in GameManager.Windows)
        {
            if (window != null && window.CollidedWindows.Contains(this))
            {
                window.CollidedWindows.Remove(this);
            }
        }

        // Update parent counters
        Parent.WindowKillCount++;
        Parent.WindowCount--;

        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

}
