using Godot;
using System;

public partial class JumpPeakState : PlayerState
{
    public override void EnterState()
    {
        Name = "JumpPeak";
    }

    public override void ExitState()
    {
    }

    public override void Update(double delta)
    {
        Player.HorizontalMovement();
        Player.HandleWallJump();
        Player.ChangeState((Node)States.Get("Fall"));
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        Player.Animator.Play("Jump");
        Player.HandleFlipH();
    }
}
