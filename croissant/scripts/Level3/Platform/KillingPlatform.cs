using Godot;

public partial class KillingPlatform : Platform
{

	[Export] public Area2D area2D;
    public override void _Ready()
    {
        base._Ready();
		area2D.BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is PlayerCharacter player)
        {
            GD.Print("Oh non je suis mort !");
        }
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
    