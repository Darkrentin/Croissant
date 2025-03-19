using Godot;
using System;

public static class States
{

    [Export] private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("uid://wm3w6j1qernu");
    [Export] private static PackedScene Level1Scene = ResourceLoader.Load<PackedScene>("uid://cppwo1k4kuwg2");
    [Export] private static PackedScene Level2Scene = ResourceLoader.Load<PackedScene>("uid://d13xxxigq3m7y");

    public static void StateDebug()
    {
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

    public static void Debug()
    {

    }
}