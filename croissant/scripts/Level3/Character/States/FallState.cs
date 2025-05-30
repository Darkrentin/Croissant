using Godot;
using System;

public partial class FallState : PlayerState
{
    [Export] public float WallSlideGravityScale = 0.3f;
    public override void EnterState()
    {
        Name = "Fall";

        Player._fallEnterMsec = (int)Time.GetTicksMsec();
        Player.StopWalkingEffects();
    }

    public override void Update(double delta)
    {
        float gravityScale = 1.0f;

        if (Player.IsOnWall())
        {
            gravityScale = WallSlideGravityScale;
            Player.HandleGravityWallFall(delta, PlayerCharacter.GravityFall * gravityScale);
            Player.HandleWallSlideParticles();
        }
        else
        {
            Player.HandleGravity(delta, PlayerCharacter.GravityFall * gravityScale);
            Player.StopWalkingEffects();
        }
        Player.HorizontalMovement(PlayerCharacter.AirAcceleration, PlayerCharacter.AirDeceleration);
        Player.HandleLanding();
        Player.HandleJump();
        Player.HandleJumpBuffer();
        Player.HandleWallJump();
        Player.HandleFallAnimations();
        Player.HandleMaxFallVelocity();
    }
}
