using Godot;
using System;

public partial class RunState : PlayerState
{
    public override void EnterState()
    {
        Name = "Run";
        Player._hasJumped = false;
    }

    public override void ExitState()
    {
    }

    public override void Update(double delta)
    {
        Player.HorizontalMovement();
        Player.HandleJump();
        Player.HandleFalling();
        HandleAnimations();
        HandleIdle();
    }

    private void HandleAnimations()
    {
        Player.Animator.Play("Run");
        Player.HandleFlipH();
    }

    private void HandleIdle()
    {
        if (Player.moveDirectionX == 0)
        {
            Player.ChangeState((Node)States.Get("Idle"));
        }
    }
}
