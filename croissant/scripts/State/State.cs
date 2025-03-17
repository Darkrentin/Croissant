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
        GameManager.GameRoot.AddChild(IntroGameScene.Instantiate<Window>());

        //Change State condition
        GameManager.State=-1;
    }
}