using Godot;

public partial class DoorPlatform : Platform
{
    [Export] public Label label;
    public bool isOpen = false;
    [Export] public int nbOfFilesToOpen = 1;

    private Vector2 HiddenPosition;
    private string LabelFormat;
    private int LastFilesCollected = -1;

    public override void _Ready()
    {
        base._Ready();
        Shader = Texture.Material as ShaderMaterial;
        Shader.SetShaderParameter("window_size", window.Size);
        HiddenPosition = GameManager.ScreenSize * 3;
        LabelFormat = $"{{0}}/{nbOfFilesToOpen}";
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        int currentFiles = Level3.Instance.FilesCollected;
        if (currentFiles != LastFilesCollected)
        {
            LastFilesCollected = currentFiles;
            label.Text = string.Format(LabelFormat, currentFiles);

            if (currentFiles >= nbOfFilesToOpen && !isOpen)
            {
                isOpen = true;
                Position = HiddenPosition;
                window.Position = (Vector2I)Position;
            }
        }
    }
}
