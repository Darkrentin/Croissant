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
        
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        if (Mathf.Abs(direction.Y) > 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }
        if (Mathf.Abs(direction.X) > 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "WalkState");
        }
    }

    public override void Exit() { GD.Print("Exit Idle"); }
}
