using Godot;

public partial class DoorPlatform : Platform
{
    [Export] public Label label;
    public bool isOpen = false;
    [Export] public int nbOfFilesToOpen = 1;
    public override void _Ready()
    {
        base._Ready();
        Shader = Texture.Material as ShaderMaterial;
        Shader.SetShaderParameter("window_size", window.Size);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        label.Text = $"{Level3.Instance.FilesCollected}/{nbOfFilesToOpen}";
        if (Level3.Instance.FilesCollected >= nbOfFilesToOpen)
        {
            Position = GameManager.ScreenSize * 3;
            window.Position = (Vector2I)Position;
        }
    }
}
