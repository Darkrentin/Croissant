using Godot;
using System;

public partial class JumpState : State
{
    private bool hasLeftGround = false;

    public override void Enter()
    {
        GD.Print("Jumping");

        var player = GetParent()?.GetParent<PlayerCharacter>();
        if (player == null)
            return;

        player.Velocity = new Vector2(player.Velocity.X, PlayerCharacter.JumpVelocity);
        hasLeftGround = false;
    }

    public override void Update(float delta)
    {
        var player = GetParent()?.GetParent<PlayerCharacter>();
        if (player == null)
            return;

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        Vector2 velocity = player.Velocity;
        velocity.X = direction.X * PlayerCharacter.Speed;
        player.Velocity = velocity;

        if (!player.IsOnFloor())
            hasLeftGround = true;

        if (hasLeftGround && player.IsOnFloor())
        {
            if (Mathf.Abs(direction.X) > 0.1f)
                EmitSignal(SignalName.StateTransition, this, "WalkState");
            else
                EmitSignal(SignalName.StateTransition, this, "IdleState");
        }
    }

    public override void Exit()
    {
        GD.Print("Exit Jump");
    }
}
