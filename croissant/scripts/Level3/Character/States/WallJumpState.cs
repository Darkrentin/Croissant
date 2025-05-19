using Godot;
using System;

public partial class WallJumpState : PlayerState
{
    private Vector2 lastWallDirection;
    private bool shouldEnableWallKick;


    public override void EnterState()
    {
        Name = "WallJump";

        lastWallDirection = Player.wallDirection;

        // Est-ce que le joueur appuie sur la direction opposée ?
        bool pushingAway = false;
        if (lastWallDirection == Vector2.Left)
            pushingAway = Player.keyRight || Player.IsDirectionBuffered();
        else if (lastWallDirection == Vector2.Right)
            pushingAway = Player.keyLeft || Player.IsDirectionBuffered();


        if (pushingAway)
        {
            // Gros kick latéral, saut plus bas
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -lastWallDirection.X,
                                          PlayerCharacter.WallJumpVelocity * 0.5f);
        }
        else
        {
            // Saut plus vertical, moins de poussée latérale
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * 0.7f * -lastWallDirection.X,
                                          PlayerCharacter.WallJumpVelocity * 1.1f);
        }
        
    }


    public override void ExitState()
    {
    }

    public override void Update(double delta)
    {
        Player.GetWallDirection();
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
            Player.ChangeState((Node)States.Get("Fall"));
        }

        if (Player.wallDirection != lastWallDirection && Player.wallDirection != Vector2.Zero)
        {
            Player.ChangeState((Node)States.Get("Fall"));
        }
    }

    private void ShouldOnlyJumpButtonWallKick(bool shouldEnable)
    {
        shouldEnableWallKick = shouldEnable;
        if (shouldEnable)
        {
            if (Player.keyLeft || Player.keyRight)
            {
                Player._hasJumped = false;
                Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y);
            }
            else
            {
                if (Player.jumps < PlayerCharacter.MaxJumps)
                {
                    Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y);
                }
                else
                {
                    Player.ChangeState((Node)States.Get("Fall"));
                }
            }
        }
        else
        {
            Player.Velocity = new Vector2(PlayerCharacter.WallJumpHSpeed * -Player.wallDirection.X, Player.Velocity.Y);
        }
    }

    private void HandleWallKickMovement()
    {
        if (!Player.keyLeft && !Player.keyRight)
        {
            Player.Velocity = Player.Velocity.MoveToward(new Vector2(0, Player.Velocity.Y), PlayerCharacter.WallJumpAcceleration);
        }
        else
        {
            if (lastWallDirection == Vector2.Left && Player.keyRight)
            {
                Player.HorizontalMovement(PlayerCharacter.WallJumpAcceleration, PlayerCharacter.WallJumpAcceleration);
            }
            else if (lastWallDirection == Vector2.Right && Player.keyLeft)
            {
                Player.HorizontalMovement(PlayerCharacter.WallJumpAcceleration, PlayerCharacter.WallJumpAcceleration);
            }
        }
    }
}