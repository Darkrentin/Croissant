using Godot;
using System;

public partial class BossFloor : Node3D
{
    [Export] public AnimationPlayer animationPlayer;
    [Export] public AnimationTree animationTree;
    public AnimationNodeStateMachinePlayback animationStateMachine;
    public Timer timer;
    private Action currentCallback;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = new Timer();
        timer.WaitTime = 0.5f;
        timer.OneShot = true;
        AddChild(timer);
        animationStateMachine = (AnimationNodeStateMachinePlayback)(animationTree.Get("parameters/playback"));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }


}
