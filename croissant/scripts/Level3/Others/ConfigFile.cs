using Godot;

public partial class ConfigFile : Area2D
{
    public static int count = 0;
    [Export] public Label FileName;

    public override void _Ready()
    {
        if (GetIndex() == 0)
        {
            count = 0;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        CheckForPlayerCollision();
    }

    private void CheckForPlayerCollision()
    {
        var overlappingBodies = GetOverlappingBodies();

        foreach (var body in overlappingBodies)
        {
            if (body is PlayerCharacter)
            {
                count++;
                Level3.Instance.CollectFile();
                QueueFree();
                break;
            }
        }
    }
}
