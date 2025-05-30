using Godot;

public partial class RunState : PlayerState
{
    private Vector2 lastPosition = Vector2.Zero;

    public override void EnterState()
    {
        Name = "Run";
        Player._hasJumped = false;
        Player.WalkParticles.Position = Vector2.Zero;
        lastPosition = Player.GlobalPosition;
        Player.WalkParticles.Emitting = true;
    }

    public override void ExitState()
    {
        Player.StepTimer.Stop();
        Player.WalkParticles.Emitting = false;
    }

    public override void Update(double delta)
    {
        Player.HorizontalMovement();
        Player.HandleJump();
        Player.HandleFalling();
        HandleParticles();
        HandleAnimations();
        HandleIdle();
        Player.HandleGravity(delta);
    }

    private void HandleParticles()
    {
        Vector2 currentPosition = Player.GlobalPosition;
        bool isActuallyMoving = (currentPosition - lastPosition).LengthSquared() > 0.1f;
        Player.WalkParticles.Emitting = Player.IsOnFloor() && isActuallyMoving && Player.moveDirectionX != 0;
        if (Player.IsOnFloor())
        {
            if (Player.StepTimer.IsStopped())
                Player.StepTimer.Start();
        }
        else
        {
            Player.StepTimer.Stop();
        }

        lastPosition = currentPosition;
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
