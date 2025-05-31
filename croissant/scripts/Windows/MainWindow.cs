using Godot;
using System.Text;

public partial class MainWindow : FloatWindow
{
    public static TextureRect FakeBackground;
    public static CanvasLayer DebugInfo;
    public Label DebugLabel;

    private StringBuilder StringBuffer = new StringBuilder(256);
    private float DebugUpdateTimer = 0f;
    private const float DebugUpdateInterval = 0.1f;

    public override void _Ready()
    {
        base._Ready();
        FakeBackground = GetNode<TextureRect>("Main/FakeDesktop");
        FakeBackground.Size = DisplayServer.ScreenGetUsableRect().Size;

        DebugInfo = GetNode<CanvasLayer>("Main/DebugInfo");
        DebugLabel = GetNode<Label>("Main/DebugInfo/DebugLabel");

        Size = GameManager.ScreenSize;
        Position = Vector2I.Zero;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        DebugUpdateTimer += (float)delta;
        if (DebugUpdateTimer >= DebugUpdateInterval)
        {
            UpdateDebugLabel();
            DebugUpdateTimer = 0f;
        }
    }

    public override void OnClose()
    {
        GameManager.MenuWindow.Open();
    }

    void UpdateDebugLabel()
    {
        StringBuffer.Clear();
        StringBuffer.Append("FPS: ");
        StringBuffer.Append(Engine.GetFramesPerSecond());
        StringBuffer.Append("\nGame State: ");
        StringBuffer.Append(GameManager.State);
        StringBuffer.Append("\nMemory Usage: ");
        StringBuffer.Append(OS.GetStaticMemoryUsage() / 1048576);
        StringBuffer.Append("MB\nUptime: ");
        StringBuffer.Append((Time.GetTicksMsec() / 1000.0f).ToString("F1"));
        StringBuffer.Append("s\n");

        DebugLabel.Text = StringBuffer.ToString();
    }

    public override void GrabWindowFocus()
    {
        Unfocusable = false;
        base.GrabWindowFocus();
        Unfocusable = true;
    }
}
