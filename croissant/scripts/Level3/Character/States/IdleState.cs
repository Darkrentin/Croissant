using Godot;

public partial class IdleState : PlayerState
{
    public override void EnterState()
    {
        Name = "Idle";
        Player._hasJumped = false;
        Player.WalkParticles.Emitting = false;
        Player.StepTimer.Stop();
    }

    public override void Update(double delta)
    {
        Player.GetInputStates();
        Player.HandleFalling();
        Player.HandleJump();
        Player.HorizontalMovement();

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
