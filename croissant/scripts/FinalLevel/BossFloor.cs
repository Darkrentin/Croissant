using Godot;
using System;

public partial class BossFloor : Node3D
{
    [Export] public AnimationPlayer animationPlayer;
    public Timer timer;
    private Action currentCallback;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        timer = new Timer();
        timer.WaitTime = 0.5f;
        timer.OneShot = true;
        AddChild(timer);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void PlayAnimation(string animation, float time = 0.5f, bool PlayBackwards = false, bool reset = true)
    {
        const float ResetTime = 3f;

        // Clear any existing callbacks by disconnecting them
        if (currentCallback != null)
        {
            timer.Timeout -= currentCallback;
            currentCallback = null;
        }

        timer.OneShot = true;
        timer.WaitTime = time;

        // Create the new callback and store it
        currentCallback = () =>
        {
            if (PlayBackwards)
            {
                animationPlayer.PlayBackwards(animation);
                if (reset)
                    PlayAnimation(animation, ResetTime, false, false);
            }
            else
            {
                animationPlayer.Play(animation);
                if (reset)
                    PlayAnimation(animation, ResetTime, true, false);
            }
        };

        // Connect the new callback
        timer.Timeout += currentCallback;
        timer.Start();
    }


}
