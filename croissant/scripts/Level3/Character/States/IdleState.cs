using Godot;
using System;

public partial class IdleState : PlayerState
{
    public override void EnterState()
    {
        Name = "Idle";
        Player._hasJumped = false;
    }

    public override void ExitState()
    {
    }

    public override void Update(double delta)
    {
        Player.GetInputStates();
        Player.HandleFalling();
        Player.HandleJump();
        Player.HorizontalMovement();

        //GD.Print($"[Idle] moveDirectionX: {Player.moveDirectionX}");

        if (Player.moveDirectionX != 0)
        {
            Player.ChangeState((Node)States.Get("Run"));
        }

        HandleAnimations();
    }

    private void HandleAnimations()
    {
        Player.Animator.Play("Idle");
        Player.HandleFlipH();
    }
}
