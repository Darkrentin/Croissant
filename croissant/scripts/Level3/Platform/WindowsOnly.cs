using Godot;
using System;

public partial class WindowsOnly : Platform
{
    public override void _Ready()
    {
        base._Ready();
        Shader.SetShaderParameter("window_size", window.Size);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }
}
