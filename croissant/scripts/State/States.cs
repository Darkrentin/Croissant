using Godot;

public static class States
{
    public static SceneLoader SceneLoader = ResourceLoader.Load<PackedScene>("res://scenes/Other/SceneLoader.tscn").Instantiate<SceneLoader>();
    public static PackedScene DifficultyScene = SceneLoader.DifficultyScene;
    public static PackedScene IntroGameScene = SceneLoader.IntroGameScene;
    public static PackedScene Level1Scene = SceneLoader.Level1Scene;
    public static PackedScene Level2Scene = SceneLoader.Level2Scene;
    public static PackedScene VirusScene = SceneLoader.VirusScene;
    public static PackedScene HelperScene = SceneLoader.HelperScene;
    public static PackedScene StaticWindowScene = SceneLoader.StaticWindowScene;
    public static PackedScene TimerWindowScene = SceneLoader.TimerWindowScene;
    public static PackedScene MoveWindowScene = SceneLoader.MoveWindowScene;
    public static PackedScene DodgeWindowScene = SceneLoader.DodgeWindowScene;
    public static PackedScene TankWindowScene = SceneLoader.TankWindowScene;
    public static PackedScene BombWindowScene = SceneLoader.BombWindowScene;
    public static PackedScene VirusSplashScene = SceneLoader.VirusSplashScene;
    public static PackedScene BsodScene = SceneLoader.BsodScene;
    public static PackedScene CursorWindowScene = SceneLoader.CursorWindowScene;
    public static PackedScene ScoreboardScene = SceneLoader.ScoreboardScene;
    public static Window Bsod;
    public static Window IntroLvl;
    public static Node Lvl1;
    public static Node Lvl2;
    public static int LevelOfTuto = 0;

    public static void Virus()
    {
        GameManager.virus.Position = Lib.GetScreenPosition(0.5f, 0.5f) - GameManager.virus.Size / 2;
        GameManager.virus.AnimationScreen.Travel("PowerOn");
        GameManager.virus.On = true;

        //change state condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void Helper()
    {
        GameManager.helper.Position = Lib.GetScreenPosition(0.5f, 0.5f) - GameManager.helper.Size / 2;
        GameManager.helper.ShowNpc(GameManager.helper.LeftDown);
        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void IntroGame()
    {
        IntroLvl = IntroGameScene.Instantiate<Window>();
        IntroLvl.Position = new Vector2I(1, 0);
        GameManager.GameRoot.AddChild(IntroLvl);

        //Change State condition
        GameManager.State = GameManager.GameState.IntroGame_Process;
    }

    public static void IntroVirus()
    {
        GameManager.virus.Position = Lib.GetScreenPosition(0.5f, -0.5f) - GameManager.virus.Size / 2;
        ////Lib.Print(virus.Position.ToString());
        GameManager.virus.StartInverseExponentialTransition(Lib.GetScreenPosition(0.5f, 1f) - new Vector2I(GameManager.virus.Size.X / 2, GameManager.virus.Size.Y), 2f);
        GameManager.virus.On = false;
        GameManager.virus.Visible = true;
        //change state condition
        GameManager.State = GameManager.GameState.IntroVirusBuffer;
    }

    public static void Dialogue1()
    {
        //Freeze the game
        if (IntroGameManager.Instance != null)
            IntroGameManager.Instance.GameNode.Visible = false;

        GameManager.virus.Position = Lib.GetScreenPosition(0.5f, 1f) - new Vector2I(GameManager.virus.Size.X / 2, GameManager.virus.Size.Y);

        ////Lib.Print("First Virus Dialogue");
        GameManager.virus.Dialogue.StartDialogue("Virus", "sleep");

        GameManager.virus.Splash();

        GameManager.virus.StartShake(0.5f, 10);

        //change state condition
        GameManager.State = GameManager.GameState.Dialogue1Buffer;
    }

    public static void VirusTutoSelection()
    {
        GameManager.virus.ForceDialoguePlacement = false;
        GameManager.virus.Position = GameManager.virus.RightDown;
        GameManager.virus.Dialogue.LockSkip = true;
        switch (LevelOfTuto)
        {
            case 0:
                VirusTuto.Tuto1();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto1");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 1:
                VirusTuto.Tuto2();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto2");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 2:
                VirusTuto.Tuto3();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto3");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 3:
                VirusTuto.Tuto4();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto4");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 4:
                VirusTuto.Tuto5();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto5");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 5:
                VirusTuto.Tuto6();
                GameManager.virus.Dialogue.StartDialogue("Virus", "tuto6");
                GameManager.State = GameManager.GameState.Void;
                break;
            case 6:
                GameManager.virus.Dialogue.LockSkip = false;
                GameManager.virus.Dialogue.StartDialogue("Virus", "tutoEnd");
                GameManager.State = GameManager.GameState.Void;
                break;
            default:
                GameManager.State = GameManager.GameState.Void;
                break;
        }
    }

    public static void Level1()
    {
        GameManager.virus.HideNpc(1);
        Lvl1 = Level1Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Lvl1);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void BlueScreen()
    {
        BlueScreenManager.ManageBlueScreen();
    }

    public static void IntroHelper()
    {
        GameManager.helper.Position = Lib.GetScreenPosition(-0.5f, 1);
        GameManager.helper.ShowNpc(GameManager.helper.LeftDown);
        GameManager.helper.DialogueToPlayAfterTransition = "Restart";

        GameManager.virus.Position = Lib.GetScreenPosition(1, -0.5f) - GameManager.virus.Size;

        //Change State condition
        GameManager.State = GameManager.GameState.IntroHelperBuffer;
    }
    public static void Level2()
    {
        GameManager.helper.HideNpc(3);
        Node2D Level2 = Level2Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Level2);

        //remove the virus
        GameManager.virus.HideNpc(1);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void Dialogue2()
    {
        GameManager.virus.ShowNpc(GameManager.virus.RightDown);
        GameManager.virus.DialogueToPlayAfterTransition = "EndLvl2";
        GameManager.State = GameManager.GameState.Void;
    }

    public static void level3()
    {
        Window Level3 = SceneLoader.Level3Scene.Instantiate<Window>();
        GameManager.GameRoot.AddChild(Level3);

        Vector2I newSize = new Vector2I(100, 100);
        GameManager.helper.GrabFocus();
        GameManager.helper.StartTransition(GameManager.ScreenSize / 2 - GameManager.helper.Size / 2, 1f);
        GameManager.helper.StartResize(newSize, 1f);
        GameManager.helper.TransitionTag = "Level3Spawn";

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void Dialogue3()
    {
        GameManager.GameRoot.GetTree().CreateTimer(1f).Timeout += () =>
        {
            GameManager.helper.Visible = true;
            GameManager.helper.Position = -GameManager.ScreenSize;
            Vector2I DialoguePosition = GameManager.ScreenSize / 2 - GameManager.helper.Dialogue.Size / 2 + new Vector2I(0, GameManager.ScreenSize.Y / 4);
            GameManager.helper.Dialogue.StartDialogue(GameManager.helper.NpcName, "EndLvl3", DialoguePosition);

        };
        GameManager.State = GameManager.GameState.Void;
    }

    public static void FinalLevel()
    {
        if (GameManager.virus != null)
        {
            GameManager.GameRoot.RemoveChild(GameManager.virus);
            GameManager.virus.QueueFree();
            GameManager.virus = null;
        }

        Node3D FinalLevel = SceneLoader.FinalLevelScene.Instantiate<Node3D>();
        GameManager.GameRoot.AddChild(FinalLevel);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void Scoreboard()
    {
        Window Scoreboard = SceneLoader.ScoreboardScene.Instantiate<Window>();
        Scoreboard.Position = GameManager.ScreenSize / 2 - Scoreboard.Size / 2;
        GameManager.GameRoot.AddChild(Scoreboard);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void IntroGame_Process(double delta)
    {
        //Change State condition
        if (IntroGameManager.score >= IntroGameManager.Instance.MaxScore)
        {
            IntroGameManager.EndActions();
            GameManager.State = GameManager.GameState.IntroVirus;
        }
    }

    public static void Debug()
    {

    }
}
