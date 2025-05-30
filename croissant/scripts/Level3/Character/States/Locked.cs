using Godot;

public partial class Locked : PlayerState
{
    private Vector2 stuckWallDir;

    public override void EnterState()
    {
        Name = "Locked";
        Player._hasJumped = false;

        // Stop horizontal movement and apply a small push to stay attached
        Player.Velocity = new Vector2(0, PlayerCharacter.GravityWallSlide * 3);

        Player.jumps = PlayerCharacter.MaxJumps;

        // Record the wall direction immediately
        Player.GetWallDirection();
        stuckWallDir = Player.wallDirection;

        // Clear 'just pressed' states to prevent immediate trigger
        // Godot doesn't allow resetting Input directly, but we can
        // consume the next press by ignoring all IsActionPressed()
        // until a new JustPressed().
    }

    public override void Update(double delta)
    {
        // Update wall direction
        Player.GetWallDirection();
        var dir = Player.wallDirection;

        // Slow descent
        Player.HandleGravity(delta, PlayerCharacter.GravityWallSlide);

        // Animation
        Player.Animator.Play("WallSlideLeft");
        Player.Sprite.FlipH = (dir == Vector2.Right);

        if (Input.IsActionJustPressed("ui_up"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }
        if (stuckWallDir == Vector2.Left && Input.IsActionJustPressed("ui_right"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }
        if (stuckWallDir == Vector2.Right && Input.IsActionJustPressed("ui_left"))
        {
            Player.ChangeState((Node)States.Get("WallJump"));
            return;
        }

        // As soon as the floor is touched or the wall is released, switch to falling
        if (dir == Vector2.Zero || Player.IsOnFloor())
        {
            Player.ChangeState((Node)States.Get("Fall"));
            return;
        }
    }
}
