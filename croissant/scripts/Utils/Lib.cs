using Godot;
using System;
using System.Runtime.CompilerServices;

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
        return (float)rand.NextDouble() * (max - min) + min;
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

    public static Vector2I GetScreenPosition(Vector2 relativePosition)
    {
        return new Vector2I(
            (int)(GameManager.ScreenSize.X * relativePosition.X),
            (int)(GameManager.ScreenSize.Y * relativePosition.Y)
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

    public static Vector2I GetScreenSize(Vector2 relativeSize)
    {
        return new Vector2I(
            (int)(GameManager.ScreenSize.X * relativeSize.X),
            (int)(GameManager.ScreenSize.Y * relativeSize.Y)
        );
    }

    public static Vector2I GetRandomPositionOutsideScreen(int side = 0, int margin = 50)
    {
        Vector2I screenSize = GameManager.ScreenSize;
        Vector2I position = new Vector2I();

        // Decide which side of the screen to place the position (0=top, 1=right, 2=bottom, 3=left)
        switch (side)
        {
            case 0: // Top
                position.X = rand.Next(-margin, screenSize.X + margin);
                position.Y = -margin;
                break;
            case 1: // Right
                position.X = screenSize.X + margin;
                position.Y = rand.Next(-margin, screenSize.Y + margin);
                break;
            case 2: // Bottom
                position.X = rand.Next(-margin, screenSize.X + margin);
                position.Y = screenSize.Y + margin;
                break;
            case 3: // Left
                position.X = -margin;
                position.Y = rand.Next(-margin, screenSize.Y + margin);
                break;
        }

        return position;
    }

    public static Vector2 GetAspectFactor(Vector2I originalSize)
    {
        Vector2 reference = new Vector2(1920, 1080);
        Vector2 scale = new Vector2(GameManager.ScreenSize.X / reference.X, GameManager.ScreenSize.Y / reference.Y);
        float scaleFactor = Mathf.Min(scale.X, scale.Y);
        return new Vector2(originalSize.X, originalSize.Y) * scaleFactor;
    }

    public static void Print(string msg, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "")
    {
        string fileName = System.IO.Path.GetFileName(filePath);
        GD.Print($"[{fileName}][{methodName}] {msg}");
    }

    public static string GetCursedString()
    {
        string s = "";
        int rand = Lib.rand.Next(0, 30);
        for (int i = 0; i < rand; i++)
        {
            s += (char)Lib.rand.Next(33, 592);
        }
        return s;

    }
}