using Godot;
using System;

public partial class TestWindow : FloatWindow
{

    private Vector2I velocity;
    private int speed = 5;
    public override void _Ready()
    {
        base._Ready();
        window.TransparentBg = true;
        velocity = new Vector2I(1, 1);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        Vector2 mouse_pos = GetViewport().GetMousePosition();
        if(Math.Abs(mouse_pos.X)>speed || Math.Abs(mouse_pos.Y)>speed)
        {
            velocity = (Vector2I)((mouse_pos).Normalized() * speed);
            WindowPosition += velocity;
            GD.Print("Pos "+mouse_pos);
        }
    }
}
