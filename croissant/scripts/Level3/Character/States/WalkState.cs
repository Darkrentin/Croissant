using Godot;
using System;

public partial class WalkState : State
{
    public override void Enter()
    {
        GD.Print("Walking");
    }

    public override void Update(float delta)
    {
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player == null)
            return;

        if (!player.IsOnFloor())
        {
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }

        Vector2 direction = new Vector2();

        if (Input.IsActionPressed("ui_left"))
            direction.X -= 1;
        if (Input.IsActionPressed("ui_right"))
            direction.X += 1;

        if (direction.X == 0)
        {
            EmitSignal(SignalName.StateTransition, this, "IdleState");
            return;
        }

        if (Input.IsActionJustPressed("ui_accept"))
        {
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }

        Vector2 velocity = player.Velocity;
        velocity.X = direction.X * PlayerCharacter.Speed;
        player.Velocity = velocity;

    }

    public override void Exit() { GD.Print("Exit Walk");}

}
