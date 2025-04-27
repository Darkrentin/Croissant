using System.Collections.Generic;
using Godot;

public static class BlueScreenManager
{
    public static List<Window> CrashWindows = new List<Window>();
    public const int MaxWindows = 25;
    public static int NbOfFakeWindow = 0;
    public static int state = 0;

    public static void ManageBlueScreen()
    {
        if (state == 0)
        {
            if (NbOfFakeWindow > MaxWindows)
            {
                state = 1;
                return;
            }
            Window window = States.StaticWindowScene.Instantiate<Window>();
            window.AlwaysOnTop = false;
            GameManager.GameRoot.AddChild(window);
            CrashWindows.Add(window);
            NbOfFakeWindow++;
        }
        if (state == 1)
        {
            States.Bsod = States.BsodScene.Instantiate<Window>();
            States.Bsod.Position = new Vector2I(0, 0);
            GameManager.GameRoot.AddChild(States.Bsod);
            States.Bsod.GrabFocus();
            GameManager.virus.HideNpc();
            state = 2;
            return;
        }
        if (state == 2)
        {
            States.Lvl1.QueueFree();
            state = 3;
            return;
        }
        if (state == 3)
        {
            for (int i = 0; i < CrashWindows.Count; i++)
            {
                CrashWindows[i].QueueFree();
            }
            CrashWindows.Clear();
            NbOfFakeWindow = 0;
            GameManager.State = GameManager.GameState.Void;
            return;
        }
    }
}