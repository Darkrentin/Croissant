using Godot;
using System;

public partial class FloatWindow : Window
{
    public override void _Ready()
    {
        Borderless = true;
    }

    public void _on_button_pressed_B()
    {
        Borderless = !Borderless;
    }

    public void _on_button_pressed_T()
    {
        TransparentBg = !TransparentBg;
        Transparent = !Transparent;
    }

    public void _on_button_pressed_R()
    {
        Unresizable = !Unresizable;
        
    }
}
