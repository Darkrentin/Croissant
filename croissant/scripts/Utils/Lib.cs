using Godot;
using System;
using System.Runtime.CompilerServices;

public static class Lib
{
    public static Random rand = new Random();

    public static Vector2I GetCursorPosition()
    {
        return (Vector2I)GameManager.FixWindow.GetMousePosition();
    }

    public static float GetRandomNormal(float min = 0, float max = 1)
    {
        return (float)rand.NextDouble() * (max - min) + min;
    }

    public static Vector2I GetScreenPosition(float relativeX, float relativeY)
    {
        return new Vector2I((int)(GameManager.ScreenSize.X * relativeX), (int)(GameManager.ScreenSize.Y * relativeY));
    }

    public static Vector2I GetScreenPosition(Vector2 relativePosition)
    {
        return new Vector2I((int)(GameManager.ScreenSize.X * relativePosition.X), (int)(GameManager.ScreenSize.Y * relativePosition.Y));
    }

    public static Vector2I GetScreenSize(float relativeWidth, float relativeHeight)
    {
        return new Vector2I((int)(GameManager.ScreenSize.X * relativeWidth), (int)(GameManager.ScreenSize.Y * relativeHeight));
    }

    public static Vector2I GetScreenSize(Vector2 relativeSize)
    {
        return new Vector2I((int)(GameManager.ScreenSize.X * relativeSize.X), (int)(GameManager.ScreenSize.Y * relativeSize.Y));
    }

    public static Vector2I GetRandomPositionOutsideScreen(int side = 0, int margin = 50)
    {
        if (side == -1)
            side = rand.Next(0, 4);
        Vector2I screenSize = GameManager.ScreenSize;

        return side switch
        {
            0 => new Vector2I(rand.Next(-margin, screenSize.X + margin), -margin), // Top
            1 => new Vector2I(screenSize.X + margin, rand.Next(-margin, screenSize.Y + margin)), // Right
            2 => new Vector2I(rand.Next(-margin, screenSize.X + margin), screenSize.Y + margin), // Bottom
            3 => new Vector2I(-margin, rand.Next(-margin, screenSize.Y + margin)), // Left
            _ => Vector2I.Zero
        };
    }

    public static Vector2I GetAspectFactor(Vector2I originalSize)
    {
        const float referenceWidth = 1920f;
        const float referenceHeight = 1080f;

        float scaleFactor = Mathf.Min(GameManager.ScreenSize.X / referenceWidth, GameManager.ScreenSize.Y / referenceHeight);

        return (Vector2I)((Vector2)originalSize * scaleFactor);
    }

    public static Vector2 GetScreenRatio()
    {
        const float referenceWidth = 1920f;
        const float referenceHeight = 1080f;

        return new Vector2(GameManager.ScreenSize.X / referenceWidth, GameManager.ScreenSize.Y / referenceHeight);
    }

    public static void Print(string msg, [CallerFilePath] string filePath = "", [CallerMemberName] string methodName = "")
    {
        string fileName = System.IO.Path.GetFileName(filePath);
        GD.Print($"[{fileName}][{methodName}] {msg}");
    }

    public static string GetCursedString()
    {
        int randLength = rand.Next(0, 30);
        char[] chars = new char[randLength];

        for (int i = 0; i < randLength; i++)
        {
            chars[i] = (char)rand.Next(33, 592);
        }

        return new string(chars);
    }

    public static float Distance(Vector2I a, Vector2I b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
    public static float Distance(Vector2 a, Vector2 b)
    {
        return (float)Math.Sqrt(Math.Pow(a.X - b.X, 2) + Math.Pow(a.Y - b.Y, 2));
    }
}
