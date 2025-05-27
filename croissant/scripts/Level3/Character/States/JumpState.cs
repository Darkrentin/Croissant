using Godot;
using System;

public partial class JumpState : PlayerState
{
    public override void EnterState()
    {
        Name = "Jump";
        Player.Velocity = new Vector2(Player.Velocity.X, Player.jumpSpeed);
        Player._hasJumped = true;
        Player.WalkParticles.Emitting = false;
        Player.SpawnJumpParticles(Vector2.Up);
    }

    public override void ExitState()
    {
    }

    public override void Update(double delta)
    {
        Player.HandleGravity(delta);
        Player.HorizontalMovement();
        Player.HandleWallJump();
        HandleJumpToFall();
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        Player.Animator.Play("Jump");
        Player.HandleFlipH();
    }

    private void HandleJumpToFall()
    {
        if (Player.Velocity.Y >= 0)
        {
            Player.ChangeState((Node)States.Get("JumpPeak"));
        }
        else if (!Player.keyJump)
        {
            Player.Velocity = new Vector2(Player.Velocity.X, Player.Velocity.Y * PlayerCharacter.VariableJumpMultiplier);
            Player.ChangeState((Node)States.Get("JumpPeak"));
        }
    }
}
