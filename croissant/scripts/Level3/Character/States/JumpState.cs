using Godot;
using System;

public partial class JumpState : State
{
    public override void Enter()
    {
        GD.Print("Jumping");

        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player == null)
            return;

        player.Velocity = new Vector2(player.Velocity.X, PlayerCharacter.JumpVelocity);
    }

    public override void Update(float delta)
    {
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player == null)
            return;

        if (player.IsOnFloor())
        {
            Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

            if (Mathf.Abs(direction.X) > 0.1f)
                EmitSignal(SignalName.StateTransition, this, "WalkState");
            else
                EmitSignal(SignalName.StateTransition, this, "IdleState");
        }
    }

    public override void Exit() { GD.Print("Exit Jump"); }
}
