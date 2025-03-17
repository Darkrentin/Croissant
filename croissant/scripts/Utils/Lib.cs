using Godot;
using System;

public static class Lib
{

    public static Random rand = new Random();
    //Get the mouse position relative to the screen size
    //this function work with the FixWindow to get the right mouse position
    //because the GetMousePosition() function only give the mouse position relative to the window that called it
    public static Vector2I GetCursorPosition()
    {
        return (Vector2I)GameManager.FixWindow.GetMousePosition();
    }

    public static float GetRandomNormal(float min = 0, float max = 1)
    {
        return (float)rand.NextDouble()* (max-min) + min;
    }

    //Get a Position on the screen based on a relative position
    //relativeX and relativeY are values between 0.0 and 1.0
    //this function allows you to work with relative positions and not absolute positions to make the game resolution independent
    public static Vector2I GetScreenPosition(float relativeX, float relativeY)
    {
        return new Vector2I(
            (int)(GameManager.ScreenSize.X * relativeX),
            (int)(GameManager.ScreenSize.Y * relativeY)
        );
    }

    //Get a Size on the screen based on a relative size
    //relativeWidth and relativeHeight are values between 0.0 and 1.0
    //this function allows you to work with relative sizes and not absolute sizes to make the game resolution independent
    public static Vector2I GetScreenSize(float relativeWidth, float relativeHeight)
    {
        return new Vector2I(
            (int)(GameManager.ScreenSize.X * relativeWidth),
            (int)(GameManager.ScreenSize.Y * relativeHeight)
        );
    }

}