using Godot;
using System;

public partial class MainWindow : FloatWindow
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        Lib.Print("MainWindow is ready!");
        //Set the main window properties
        Position = new Vector2I(0, 0);
        ContentScaleSize = new Vector2I(1920, 1080);
        ContentScaleMode = ContentScaleModeEnum.CanvasItems;
        ContentScaleAspect = ContentScaleAspectEnum.Expand;
        //Size = DisplayServer.ScreenGetUsableRect().Size;
        Size = GameManager.ScreenSize;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void OnClose()
    {
        Lib.Print("MainWindow is closing!");
        GameManager.MenuWindow.Open();
    }
}