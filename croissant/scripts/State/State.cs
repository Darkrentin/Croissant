using Godot;
using System;
using System.Drawing;

public static class States
{

    [Export] private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("uid://wm3w6j1qernu");
    [Export] private static PackedScene Level1Scene = ResourceLoader.Load<PackedScene>("uid://cppwo1k4kuwg2");
    [Export] private static PackedScene Level2Scene = ResourceLoader.Load<PackedScene>("uid://d13xxxigq3m7y");
    [Export] private static PackedScene VirusScene = ResourceLoader.Load<PackedScene>("uid://cbd8iklbee2ig");

    public static void IntroAnimation()
    {
        Virus virus = VirusScene.Instantiate<Virus>();
        GameManager.virus = virus;
        GameManager.GameRoot.AddChild(virus);
        virus.Position = Lib.GetScreenPosition(0.5f, -0.5f)- virus.Size/2;
        Lib.Print(virus.Position.ToString());
        virus.StartInverseExponentialTransition(Lib.GetScreenPosition(0.5f, 1f)-new Vector2I(virus.Size.X/2,virus.Size.Y),2f);

        //change state condition
        GameManager.State = GameManager.GameState.IntroBuffer;
    }

    public static void FirstVirusDialogue()
    {
        Lib.Print("First Virus Dialogue");
        GameManager.virus.dialogue.StartDialogue("Virus", "sleep");
        Node2D explosion = GameManager.virus.ExplosionScene.Instantiate<Node2D>();
        explosion.Position = GameManager.virus.Position + new Vector2I(GameManager.virus.Size.X / 2, GameManager.virus.Size.Y);
        GameManager.GameRoot.AddChild(explosion);

        //change state condition
        GameManager.State = GameManager.GameState.Debug;

    }
    public static void Intro()
    {
        Window IntroGame = IntroGameScene.Instantiate<Window>();
        IntroGame.Position = new Vector2I(1, 0);
        GameManager.GameRoot.AddChild(IntroGame);


        //Change State condition
        GameManager.State = GameManager.GameState.Debug;
    }

    public static void Level1()
    {
        Node2D Level1 = Level1Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Level1);

        //Change State condition
        GameManager.State = GameManager.GameState.Debug;
    }
    public static void Level2()
    {
        Node2D Level2 = Level2Scene.Instantiate<Node2D>();
        GameManager.GameRoot.AddChild(Level2);

        //Change State condition
        GameManager.State = GameManager.GameState.Debug;
    }

    public static void StateDebug()
    {

    }
}