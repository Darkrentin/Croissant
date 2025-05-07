using Godot;

public partial class MovePlatform : Platform
{
    public override void _Ready()
    {
        base._Ready();
        Freeze = false;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
    