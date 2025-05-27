using Godot;
using System;

public partial class WallJumpState : PlayerState
{
    private Vector2 lastWallDirection;
    private bool shouldEnableWallKick;
    private bool HasJumped = false;


    public override void EnterState()
    {
        Name = "WallJump";
        Player.Velocity = new Vector2(Player.Velocity.X * 1.5f, Player.WallJumpVelocity);
        lastWallDirection = Player.wallDirection;
        HasJumped = false;
        Player.WalkParticles.Emitting = false;
        Player.WalkParticles.Position = Vector2.Zero;
        Player.SpawnWallJumpParticles(Player.wallDirection);
        KickOut();
        //GD.Print("Last wall direction: " + lastWallDirection);
    }

    public override void Update(double delta)
    {
        Player.HandleGravity(delta, PlayerCharacter.GravityJump);
        HandleWallKickMovement();
        HandleWallJumpEnd(delta);
        HandleAnimations();
    }

    private void HandleAnimations()
    {
        Player.Animator.Play("Jump");
        Player.Sprite.FlipH = Player.Velocity.X < 1;
    }

    private void HandleWallJumpEnd(double delta)
    {
        if (Player.Velocity.Y >= PlayerCharacter.WallJumpYSpeedPeak)
        {
            if (Player.Velocity.Y < 0) Player.Velocity = new Vector2(Player.Velocity.X, 0);
            Player.ChangeState((Node)States.Get("Fall"));
        }

        if (Player.wallDirection != lastWallDirection && Player.wallDirection != Vector2.Zero)
        {
            Player.ChangeState((Node)States.Get("Fall"));
        }

        if (Player.IsOnWall())
        {
            if (Player.Velocity.Y < 0) Player.Velocity = new Vector2(Player.Velocity.X, 0);
            Player.ChangeState((Node)States.Get("Fall"));
        }
    }

    private void KickOut()
    {
        if (lastWallDirection == Vector2.Left && Player.keyRight)
        {
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y / 2.0f);
        }
        else if (lastWallDirection == Vector2.Right && Player.keyLeft)
        {
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y / 2.0f);
        }
        else
        {
            HasJumped = true;
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X / 2.8f, Player.jumpSpeed);
        }
    }

    private void HandleWallKickMovement()
    {
        if (!HasJumped)
        {
            if (Player.keyJump)
            {
                Player.Velocity = new Vector2(Player.Velocity.X / 1.5f, Player.jumpSpeed);
                HasJumped = true;
            }
        }

        if (HasJumped)
        {
            if (lastWallDirection == Vector2.Left && Player.keyRight)
            {
                Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y);
            }
            else if (lastWallDirection == Vector2.Right && Player.keyLeft)
            {
                Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y);
            }
        }
    }
}
