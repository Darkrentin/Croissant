using Godot;
using System;

public partial class IdleState : State
{
    public override void Enter()
    {
        GD.Print("Idle");
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player != null)
        {
            Vector2 velocity = player.Velocity;
            velocity.X = 0;
            player.Velocity = velocity;
        }
    }

    public override void Update(float delta)
    {
        var player = GetParent().GetParent<PlayerCharacter>();
        if (player == null)
            return;
        
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");

        if (Mathf.Abs(direction.Y) > 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }
        if (Mathf.Abs(direction.X) > 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "WalkState");
        }

        if (!player.IsOnFloor() && player.Velocity.Y > 0)
        {
            if (player.IsOnWall())
            {
                for (int i = 0; i < player.GetSlideCollisionCount(); i++)
                {
                    KinematicCollision2D collision = player.GetSlideCollision(i);
                    Vector2 normal = collision.GetNormal();

                    if (normal.X > 0)
                    {
                        EmitSignal(SignalName.StateTransition, this, "WallSlideRight");
                        return;
                    }
                    else if (normal.X < 0)
                    {
                        EmitSignal(SignalName.StateTransition, this, "WallSlideLeft");
                        return;
                    }
                }
            }
            EmitSignal(SignalName.StateTransition, this, "FallState");
        }



        
    }

    public override void Exit() { GD.Print("Exit Idle"); }
}
