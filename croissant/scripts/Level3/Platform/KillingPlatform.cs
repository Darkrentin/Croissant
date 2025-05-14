using Godot;

public partial class KillingPlatform : Platform
{
    [Export] public Area2D area2D;

    public override void _Ready()
    {
        base._Ready();
        area2D.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayerCharacter player)
        {
            player.Death();
        }
    }
}