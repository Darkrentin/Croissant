using Godot;
using System;

public partial class MainWindow : FloatWindow
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();
        GD.Print("MainWindow is ready!");
        //Set the main window properties
        Draggable = false;
        transitionMode = FloatWindow.TransitionMode.Exponential;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    protected override void OnClose()
    {
        GD.Print("MainWindow is closing!");
        GetTree().Quit();
    }
}