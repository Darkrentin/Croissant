using Godot;
using System;

public partial class WalkState : State
{
    public override void Enter()
    {
        GD.Print("Walking");
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player != null)
        {
            Vector2 velocity = player.Velocity;
            velocity.X = 0;
            player.Velocity = velocity;
        }
        player.AnimationPlayer.Play("Run");
    }

    public override void Update(float delta)
    {
        var player = GetParent().GetParent<PlayerCharacter>();
        if (player == null)
            return;

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");
        Vector2 velocity = player.Velocity;
        velocity.X = direction.X * PlayerCharacter.Speed;
        player.Velocity = velocity;
        if(velocity.X>0)
        {
            player.Sprite.FlipH = false;
        }
        else if(velocity.X<0)
        {
            player.Sprite.FlipH = true;
        }

        if (Mathf.Abs(direction.Y) > 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }
        if (Mathf.Abs(direction.X) <= 0.1f)
        {
            EmitSignal(SignalName.StateTransition, this, "IdleState");
        }
        
        if (!player.IsOnFloor() && player.Velocity.Y > 0)
        {
            EmitSignal(SignalName.StateTransition, this, "FallState");
        }


    }

    public override void Exit() { GD.Print("Exit Walk");}

}
