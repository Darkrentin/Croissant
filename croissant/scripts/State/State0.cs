using Godot;
using System;

public static class State0
{

    private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("uid://wm3w6j1qernu");
    public static void Process()
    {
        GameManager.GameRoot.AddChild(IntroGameScene.Instantiate<Window>());

        //Change State condition
        GameManager.State=-1;
    }
}