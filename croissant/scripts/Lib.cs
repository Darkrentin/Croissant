using Godot;
using System;

public static class Lib
{
    public static Vector2 GetCursorPosition()
    {
        return GameManager.FixWindow.GetMousePosition() / GameManager.SizeRatio;
    }

        // Add these to GameManager or create a utility class
    public static Vector2I ToScreenCoordinates(Vector2I virtualCoords)
    {
        return (Vector2I)(virtualCoords * GameManager.SizeRatio);
    }

    public static Vector2I ToVirtualCoordinates(Vector2I screenCoords)
    {
        return (Vector2I)(screenCoords / GameManager.SizeRatio);
    }
}