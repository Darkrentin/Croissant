using Godot;
using System;

public partial class MainWindow : FloatWindow
{
    public static TextureRect FakeBackground;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        FakeBackground = GetNode<TextureRect>("Main/FakeDesktop");
        FakeBackground.Size = DisplayServer.ScreenGetUsableRect().Size;
        Size = GameManager.ScreenSize;
        //Set the main window properties
        Position = new Vector2I(0, 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void OnClose()
    {
        GameManager.MenuWindow.Open();
    }
}