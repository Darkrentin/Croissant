using Godot;
using System;

public partial class Platform : Node2D
{
	// Called when the node enters the scene tree for the first time.

	[Export] public Area2D area;
	[Export] public Window window;
	public bool Pressed = false;
	public Vector2I MouseOffset;
	public override void _Ready()
	{
		area.InputEvent += OnClickableAreaInputEvent;
		window.Visible = true;
	}

    private void OnClickableAreaInputEvent(Node viewport, InputEvent @event, long shapeIdx)
    {
        if (@event is InputEventMouseButton mouseButtonEvent)
        {
            // Vérifiez si c'est le bouton gauche
            if (mouseButtonEvent.ButtonIndex == MouseButton.Left)
            {
                // Vérifiez si le bouton vient d'être pressé
                if (mouseButtonEvent.Pressed)
                {
                    GD.Print("Clic gauche ENFONCÉ dans la zone !");
					Pressed = true;
					MouseOffset = Lib.GetCursorPosition() - (Vector2I)Position;
                    // Ajoutez ici le code à exécuter AU MOMENT où le clic gauche est enfoncé dans la zone
                }
                // Vérifiez si le bouton vient d'être relâché
                else // mouseButtonEvent.Pressed est faux, donc le bouton est relâché
                {
                    GD.Print("Clic gauche RELÂCHÉ dans la zone !");
					Pressed = false;
                    // Ajoutez ici le code à exécuter AU MOMENT où le clic gauche est relâché dans la zone
                }
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if(Pressed && Position != Lib.GetCursorPosition() - MouseOffset)
			Position = Lib.GetCursorPosition() - MouseOffset;
			window.Position = (Vector2I)Position;
	}
}
