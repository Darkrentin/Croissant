using Godot;
using Microsoft.VisualBasic.FileIO;
using System;

[Tool]
public partial class FloatWindow : Node2D
{
    [Export] public Window window;
    [Export] public bool isMainWindow = false;


    [Export] public Vector2I WindowPosition
    {
        get
        {
            return window.Position;
        }
        set
        {
            window.Position = SetWindowPosition(value);
        }
    }

    [Export] public Vector2I WindowSize
    {
        get
        {
            return window.Size;
        }
        set
        {
            window.Size = value;
        }
    }

    //Ready
    public override void _Ready()
    {
        if(isMainWindow){ window = GetWindow();}
        if(window == null)
        {
            GD.Print("Window is null");
            return;
        }

        WindowPosition = (Vector2I)Position;

    }

    public override void _Process(double delta)
    {
    }

    protected Vector2I SetWindowPosition(Vector2I position)
    {
        int ValidX;
        int ValidY;
        
        if(position.X<0)
        {
            ValidX = 0;
        }
        else if(position.X > GameManager.ScreenSize.X - window.Size.X)
        {
            ValidX = GameManager.ScreenSize.X - window.Size.X;
        }
        else
        {
            ValidX = position.X;
        }

        if(position.Y<0)
        {
            ValidY = 0;
        }
        else if(position.Y > GameManager.ScreenSize.Y - window.Size.Y)
        {
            ValidY = GameManager.ScreenSize.Y - window.Size.Y;
        }
        else
        {
            ValidY = position.Y;
        }

        return new Vector2I(ValidX, ValidY);
    }
}
