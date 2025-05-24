using System.Collections.Generic;
using Godot;

public static class BlueScreenManager
{
    public static List<Window> CrashWindows = new List<Window>();
    public const int MaxWindows = 25;
    public static int NbOfFakeWindow = 0;
    public static int state = 0;
    public static AudioStreamPlayer PopupEnter;
    public static bool NotInstancied = true;

    public static void ManageBlueScreen()
    {
        if (NotInstancied)
        {
            PopupEnter = new AudioStreamPlayer();
            PopupEnter.Stream = ResourceLoader.Load<AudioStream>("res://assets/sounds/level_1/popup_enter.mp3");
            PopupEnter.Bus = "SFX";
            PopupEnter.VolumeDb = -10f;
            GameManager.GameRoot.AddChild(PopupEnter);
            NotInstancied = false;
        }

        if (state == 0)
        {
            if (NbOfFakeWindow > MaxWindows)
            {
                state = 1;
                return;
            }
            PopupEnter.Play();
            StaticWindow window = States.StaticWindowScene.Instantiate<StaticWindow>();
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
            GameManager.virus.HideNpc(1);
            state = 2;
            return;
        }

        if (state == 2)
        {
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
