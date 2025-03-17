using Godot;
using System;

public static class State0
{

    private static PackedScene IntroGameScene = ResourceLoader.Load<PackedScene>("res://scenes/Intro/Game.tscn");
    public static void Process()
    {
        GameManager.GameRoot.AddChild(IntroGameScene.Instantiate<Window>());

        //Change State condition
        
    }
}