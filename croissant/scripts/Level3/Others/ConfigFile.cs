using Godot;

public partial class ConfigFile : Area2D
{
    public static int count = 0;
    [Export] public Label FileName;
    [Export] public AnimationPlayer AnimationPlayer;

    public override void _Ready()
    {
        if (GetIndex() == 0)
        {
            count = 0;
        }
        BodyEntered += OnCollision;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public void OnCollision(Node body)
    {
        if (body is PlayerCharacter p && !p.isDead)
        {
            count++;
            Level3.Instance.CollectFile();
            AnimationPlayer.Play("Collect");

            GetTree().CreateTimer(0.5f).Timeout += () =>
            {
                GetParent().RemoveChild(this);
                QueueFree();
            };
        }
    }
}
