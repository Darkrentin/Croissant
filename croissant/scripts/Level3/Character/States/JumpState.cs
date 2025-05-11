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
        player.AnimationPlayer.Play("Jump");
    }

    public override void Update(float delta)
    {
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player == null)
            return;

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        
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

        if (player.IsOnFloor())
		{
			if (Mathf.Abs(direction.X) > 0.1f)
				EmitSignal(SignalName.StateTransition, this, "WalkState");
			else
				EmitSignal(SignalName.StateTransition, this, "IdleState");
		}
		

        if (!player.IsOnFloor() && player.Velocity.Y > 0)
        {
            EmitSignal(SignalName.StateTransition, this, "FallState");
        }

    }

    public override void Exit() { }
}