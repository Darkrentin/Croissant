using Godot;
using System;

public partial class FallState : PlayerState
{
    public override void EnterState()
    {
        Name = "Fall";
        
        Player._fallEnterMsec = (int)Time.GetTicksMsec();
    }

    public override void Update(double delta)
    {


        Player.HandleGravity(delta, PlayerCharacter.GravityFall);
        Player.HorizontalMovement(PlayerCharacter.AirAcceleration, PlayerCharacter.AirDeceleration);

        
        Player.HandleLanding();
        Player.HandleJump();
        Player.HandleJumpBuffer();
        Player.HandleWallJump();

        HandleAnimations();

        
        if (Player.IsOnWall() && !Player.IsOnFloor() && Player.Velocity.Y > 0)
        {
            Player.ChangeState((Node)States.Get("Locked"));
        }
    }



    private void HandleAnimations()
    {
        Player.Animator.Play("Fall");
        Player.HandleFlipH();
    }
}
