using Godot;
using System;
using System.Drawing;

public partial class Platform : Node2D
{
	// Called when the node enters the scene tree for the first time.

	[Export] public FloatWindow window;
	public bool Pressed = false;
	public Vector2I MouseOffset;
    [Export] public AttackWindow[] attackWindows;
    public Level3 level3;
	public override void _Ready()
	{
		window.Visible = true;
        Lib.Print("titlebar height: " + window.TitleBarHeight);
        level3 = (Level3)GetParent();
        level3.MouseEvent+=MouseEvent;

	}


    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(Pressed && Position != Lib.GetCursorPosition() - MouseOffset)
			Position = Lib.GetCursorPosition() - MouseOffset;
			window.Position = (Vector2I)Position;
	}

    public void MouseEvent(InputEventMouseButton mouseButtonEvent)
    {
        Lib.Print("MouseEvent: " + mouseButtonEvent.Pressed);
        if (mouseButtonEvent.ButtonIndex == MouseButton.Left && MouseOnWindow())
        {
            // Vérifiez si le bouton vient d'être pressé
            if (mouseButtonEvent.Pressed)
            {
                GD.Print("Clic gauche ENFONCÉ dans la zone !");
				Pressed = true;
				MouseOffset = Lib.GetCursorPosition() - (Vector2I)Position;
            }
            else
            {
                GD.Print("Clic gauche RELÂCHÉ dans la zone !");
				Pressed = false;
            }
        }

    }

    public bool MouseOnWindow()
    {
        Vector2I mousePos = Lib.GetCursorPosition();
        Vector2I windowPos = window.Position;
        Vector2I windowSize = window.Size;
        int titleBarHeight = window.TitleBarHeight;


        return mousePos.X >= windowPos.X && 
               mousePos.X <= windowPos.X + windowSize.X &&
               mousePos.Y >= windowPos.Y - titleBarHeight &&
               mousePos.Y <= windowPos.Y;
    }
}
