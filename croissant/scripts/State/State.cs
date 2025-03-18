using Godot;
using System;

public static class States
{

    private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("uid://wm3w6j1qernu");

    public static void StateDebug()
    {
    }
    public static void State0()
    {
        Window IntroGame = IntroGameScene.Instantiate<Window>();
        IntroGame.Position = new Vector2I(1, 0);
        GameManager.GameRoot.AddChild(IntroGame);


        //Change State condition
        GameManager.State = -1;
    }
}