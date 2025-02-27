using Godot;
using System;

public partial class FloatWindow : Window
{

    public int speed = 10;

    public Vector2I WindowSize;

    [Export] public TextEdit textEdit;
    public override void _Ready()
    {
        WindowSize = Size;
    }

    public override void _Process(double delta)
    {
        if(Input.IsKeyPressed(Key.Q))
        {
            Position = new Vector2I(Position.X - speed, Position.Y);
        }
        if(Input.IsKeyPressed(Key.D))
        {
            Position = new Vector2I(Position.X + speed, Position.Y);
        }
        if(Input.IsKeyPressed(Key.Z))
        {
            Position = new Vector2I(Position.X, Position.Y - speed);
        }
        if(Input.IsKeyPressed(Key.S))
        {
            Position = new Vector2I(Position.X, Position.Y + speed);
        }

        GD.Print(Position);
    }

    public void _on_button_pressed_B()
    {
        Borderless = !Borderless;
    }

    public void _on_button_pressed_T()
    {
        TransparentBg = !TransparentBg;
        Transparent = !Transparent;
    }

    public void _on_button_pressed_R()
    {
        Unresizable = !Unresizable;
        
    }

    public void _on_button_pressed_A()
    {
        AlwaysOnTop = !AlwaysOnTop;
    }

    public void _on_button_pressed_S()
    {
        Size = WindowSize;
    }


}
