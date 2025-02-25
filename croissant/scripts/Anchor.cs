using Godot;

public partial class Anchor : RigidBody2D
{
    private float forceMultiplier = 500f;

    public override void _PhysicsProcess(double delta)
    {
        GlobalPosition = GetGlobalMousePosition();
    }
}
