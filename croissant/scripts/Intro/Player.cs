using Godot;

public partial class Player : Node2D
{
    public override void _Ready()
    {

    }

    public override void _Process(double delta)
    {
        // Rotate the player to face the mouse
        Rotation = GlobalPosition.AngleToPoint(GetGlobalMousePosition());
    }
}