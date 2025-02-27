using Godot;
using System;

public partial class MainWindow : TestWindow
{
    public override void _Ready()
    {
        base._Ready();
        window.TransparentBg = true;
        window.Title = "Main Window";
        window.Unresizable = true;
        window.TransparentBg = true;

        WindowSize = new Vector2I(600, 600);
        /*
        ProjectSettings.SetSetting("display/window/size/viewport_width", WindowSize.X);
        ProjectSettings.SetSetting("display/window/size/viewport_height", WindowSize.Y);
        ProjectSettings.Save();
        */
    }
}
