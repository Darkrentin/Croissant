using Godot;

public partial class MainWindow : FloatWindow
{
    public static TextureRect FakeBackground;
    public static CanvasLayer DebugInfo;
    public Label DebugLabel;

    public override void _Ready()
    {
        base._Ready();
        FakeBackground = GetNode<TextureRect>("Main/FakeDesktop");
        FakeBackground.Size = DisplayServer.ScreenGetUsableRect().Size;

        DebugInfo = GetNode<CanvasLayer>("Main/DebugInfo");

        DebugLabel = GetNode<Label>("Main/DebugInfo/DebugLabel");

        Size = GameManager.ScreenSize;
        //Set the main window properties
        Position = new Vector2I(0, 0);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        UpdateDebugLabel();
    }

    public override void OnClose()
    {
        GameManager.MenuWindow.Open();
    }

    void UpdateDebugLabel()
    {
        DebugLabel.Text = $"FPS: {Engine.GetFramesPerSecond()}\n";
        DebugLabel.Text += $"Mouse Position: {Lib.GetCursorPosition()}\n";
        DebugLabel.Text += $"Game State: {GameManager.State}\n";
        DebugLabel.Text += $"Memory Usage: {Godot.OS.GetStaticMemoryUsage() / 1024 / 1024}MB\n";
        DebugLabel.Text += $"Uptime: {Time.GetTicksMsec() / 1000.0f}s\n";


    }
}