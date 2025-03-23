using Godot;
using System;
using System.Drawing;

public static class States
{

    [Export] private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("uid://wm3w6j1qernu");
    [Export] private static PackedScene Level1Scene = ResourceLoader.Load<PackedScene>("uid://cppwo1k4kuwg2");
    [Export] private static PackedScene Level2Scene = ResourceLoader.Load<PackedScene>("uid://d13xxxigq3m7y");
    [Export] private static PackedScene VirusScene = ResourceLoader.Load<PackedScene>("uid://cbd8iklbee2ig");

    public static PackedScene StaticWindowScene = ResourceLoader.Load<PackedScene>("uid://dojmcfkfdnwsu");
    public static PackedScene TimerWindowScene = ResourceLoader.Load<PackedScene>("uid://ce1xhbt2knpmv");
    public static PackedScene MoveWindowScene = ResourceLoader.Load<PackedScene>("uid://cb1neywi8udoc");
    public static PackedScene DodgeWindowScene = ResourceLoader.Load<PackedScene>("uid://cdcpehwcb167t");
    public static PackedScene TankWindowScene = ResourceLoader.Load<PackedScene>("uid://dsmobrvf3clby");
    public static PackedScene BombWindowScene = ResourceLoader.Load<PackedScene>("uid://cjcfsjb8cgs3k");

    public static int LevelOfTuto = 0;

    public static void Virus()
    {
        Virus virus = VirusScene.Instantiate<Virus>();
        GameManager.virus = virus;
        GameManager.GameRoot.AddChild(virus);
        virus.Position = Lib.GetScreenPosition(0.5f, 0.5f) - virus.Size / 2;
        virus.AnimationScale.Play("PowerOn");
        virus.On = true;

        //change state condition
        GameManager.State = GameManager.GameState.Void;
    }
    public static void IntroGame()
    {
        Window IntroGame = IntroGameScene.Instantiate<Window>();
        IntroGame.Position = new Vector2I(1, 0);
        GameManager.GameRoot.AddChild(IntroGame);


        //Change State condition
        GameManager.State = GameManager.GameState.IntroGameProcess;
    }

    public static void IntroVirus()
    {
        Virus virus = VirusScene.Instantiate<Virus>();
        GameManager.virus = virus;
        GameManager.GameRoot.AddChild(virus);
        virus.Position = Lib.GetScreenPosition(0.5f, -0.5f) - virus.Size / 2;
        Lib.Print(virus.Position.ToString());
        virus.StartInverseExponentialTransition(Lib.GetScreenPosition(0.5f, 1f) - new Vector2I(virus.Size.X / 2, virus.Size.Y), 2f);

        //change state condition
        GameManager.State = GameManager.GameState.IntroVirusBuffer;
    }

    public static void VirusDialogue1()
    {
        //Freeze the game
        IntroGameManager.Instance.ProcessMode = IntroGameManager.ProcessModeEnum.Disabled;
        IntroGameManager.Instance.GetParent<Window>().Unfocusable = true;

        Lib.Print("First Virus Dialogue");
        GameManager.virus.dialogue.StartDialogue("Virus", "sleep");
        Node2D explosion = GameManager.virus.ExplosionScene.Instantiate<Node2D>();
        explosion.Position = GameManager.virus.Position + new Vector2I(GameManager.virus.Size.X / 2, GameManager.virus.Size.Y);
        GameManager.GameRoot.AddChild(explosion);

        //change state condition
        GameManager.State = GameManager.GameState.VirusDialogue1Buffer;


    }

    public static void VirusTuto()
    {
        switch(LevelOfTuto)
        {
            case(0):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto1");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(1):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto2");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(2):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto3");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(3):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto4");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(4):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto5");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(5):
                GameManager.virus.dialogue.StartDialogue("Virus", "tuto6");
                GameManager.State = GameManager.GameState.Void;
                break;
            case(6):
                GameManager.virus.dialogue.StartDialogue("Virus", "tutoEnd");
                GameManager.State = GameManager.GameState.Void;
                break;
            default:
                GameManager.State = GameManager.GameState.Void;
                break;
        }
    }

    public static void Level1()
    {
        Node2D Level1 = Level1Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Level1);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }
    public static void Level2()
    {
        Node2D Level2 = Level2Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Level2);

        //Change State condition
        GameManager.State = GameManager.GameState.Void;
    }

    public static void IntroGameProcess(double delta)
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