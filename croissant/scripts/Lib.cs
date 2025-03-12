using Godot;
using System;

public static class Lib
{

    public static Random rand = new Random();
    //Get the mouse position relative to the screen size
    //this function work with the FixWindow to get the right mouse position
    //because the GetMousePosition() function only give the mouse position relative to the window that called it
    public static Vector2 GetCursorPosition()
    {
        return GameManager.FixWindow.GetMousePosition();
    }

    public static float GetRandomNormal(float min = 0, float max = 1)
    {
        return (float)rand.NextDouble()* (max-min) + min;
    }

}