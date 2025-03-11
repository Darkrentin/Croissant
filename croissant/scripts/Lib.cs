using Godot;
using System;

public static class Lib
{
    public static Vector2 GetCursorPosition()
    {
        return GameManager.FixWindow.GetMousePosition();
    }
}