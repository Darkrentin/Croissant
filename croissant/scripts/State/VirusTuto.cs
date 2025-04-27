using Godot;
using System;

public static class VirusTuto
{
    public static void _Process()
    {

    }

    public static void Tuto1()
    {
        GameManager.virus.dialogue.PlaceDialogueWindow();
        StaticWindow popup = States.StaticWindowScene.Instantiate<StaticWindow>();
        GameManager.GameRoot.AddChild(popup);
        popup.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup.Size / 2;
        popup.CloseRequested -= popup.OnClose;
        popup.CloseRequested += () =>
        {
            GameManager.State = GameManager.GameState.VirusTuto;
            States.LevelOfTuto++;
            popup.QueueFree();
        };
    }

    public static void Tuto2()
    {
        TankWindow popup2 = States.TankWindowScene.Instantiate<TankWindow>();
        GameManager.GameRoot.AddChild(popup2);
        popup2.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup2.Size / 2;
        popup2.CloseRequested -= popup2.OnClose;
        popup2.CloseRequested += () =>
        {
            popup2.HPs--;
            if (popup2.HPs <= 0)
            {
                GameManager.State = GameManager.GameState.VirusTuto;
                States.LevelOfTuto++;
                popup2.QueueFree();
            }
            else
                popup2.CheckHp();
            popup2.StartShake(0.2f, 10);
        };
    }

    public static void Tuto3()
    {
        TimerWindow popup3 = States.TimerWindowScene.Instantiate<TimerWindow>();
        GameManager.GameRoot.AddChild(popup3);
        popup3.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup3.Size / 2;
        popup3.CloseRequested -= popup3.OnClose;
        popup3.CloseRequested += () =>
        {
            GameManager.State = GameManager.GameState.VirusTuto;
            States.LevelOfTuto++;
            popup3.QueueFree();
        };
        popup3.timer.Timeout -= popup3._on_timer_timeout;
        popup3.timer.Timeout += () =>
        {
            Tuto3();
            popup3.QueueFree();
        };
    }

    public static void Tuto4()
    {
        BombWindow popup4 = States.BombWindowScene.Instantiate<BombWindow>();
        GameManager.GameRoot.AddChild(popup4);
        popup4.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup4.Size / 2;
        popup4.CloseRequested -= popup4.OnClose;
        popup4.CloseRequested += () =>
        {
            Tuto4();
            popup4.QueueFree();
        };

        popup4.timer.Timeout -= popup4._on_timer_timeout;
        popup4.timer.Timeout += () =>
        {
            GameManager.State = GameManager.GameState.VirusTuto;
            States.LevelOfTuto++;
            popup4.QueueFree();
        };
    }

    public static void Tuto5()
    {
        MoveWindow popup5 = States.MoveWindowScene.Instantiate<MoveWindow>();
        GameManager.GameRoot.AddChild(popup5);
        popup5.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup5.Size / 2;
        popup5.CloseRequested -= popup5.OnClose;
        popup5.CloseRequested += () =>
        {
            GameManager.State = GameManager.GameState.VirusTuto;
            States.LevelOfTuto++;
            popup5.QueueFree();
        };
    }

    public static void Tuto6()
    {
        DodgeWindow popup6 = States.DodgeWindowScene.Instantiate<DodgeWindow>();
        GameManager.GameRoot.AddChild(popup6);
        popup6.Position = Lib.GetScreenPosition(0.5f, 0.5f) - popup6.Size / 2;
        popup6.CloseRequested -= popup6.OnClose;
        popup6.CloseRequested += () =>
        {
            GameManager.State = GameManager.GameState.VirusTuto;
            States.LevelOfTuto++;
            popup6.QueueFree();
        };
    }
}