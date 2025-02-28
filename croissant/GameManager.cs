using Godot;
using System;

public partial class GameManager : Node2D
{
    public static Vector2I ScreenSize;
    [Export] public Camera2D Camera { get; set; }
    [Export] public Node2D lvl { get; set; }

    public Window window;
    public Vector2I windowPosition = new Vector2I(500, 111);

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        ScreenSize = DisplayServer.ScreenGetSize();
        window = GetWindow();
        window.AlwaysOnTop = true;
    }

    public override void _Process(double delta)
    {
        // Called every frame. Delta is time since last frame.
        // Update game logic here.
        Camera.Position = GetWindow().Position;
        lvl.Position = GetWindow().Position;
        DisplayServer.WindowSetPosition(windowPosition);
        
    }
}
