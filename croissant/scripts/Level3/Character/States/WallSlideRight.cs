using Godot;
using System;

public partial class WallSlideRight : State
{
    [Export] public float WallSlideSpeed = 50.0f;

	[Export] public float PushOffForce = 100.0f;

    public override void Enter()
    {
        //GD.Print("WallSlideRight");
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player != null)
        {
            Vector2 velocity = player.Velocity;
            velocity.X = 0;
            player.Velocity = velocity;
        }
        player.Sprite.FlipH = true;
        player.AnimationPlayer.Play("WallSlideRight");
    }

    public override void Update(float delta)
    {
        var player = GetParent().GetParent<PlayerCharacter>();
        if (player == null)
            return;

		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");
        
        if (player.IsOnWall())
        {
            player.Velocity = new Vector2(player.Velocity.X, WallSlideSpeed * delta);
        }
        else
        {
            //EmitSignal(SignalName.StateTransition, this, "FallState");
            //return;
        }

        
        if (Input.IsActionJustPressed("ui_up"))
        {

			EmitSignal(SignalName.StateTransition, this, "JumpState");
			return;
			

        }


        if (Input.IsActionJustPressed("ui_right"))
        {
			if (player.IsOnWall())
			{
				player.Velocity = new Vector2(PushOffForce, player.Velocity.X);
				return;
			}
			else
			{
				EmitSignal(SignalName.StateTransition, this, "FallState");
				return;
			}
		}
        
        if (player.IsOnFloor())
        {
            
            if (Mathf.Abs(direction.X) <= 0.1f)
            {
				player.Velocity = new Vector2(PushOffForce, player.Velocity.X);
                EmitSignal(SignalName.StateTransition, this, "IdleState");
            }
            
            else
            {
				player.Velocity = new Vector2(PushOffForce, player.Velocity.X);
                EmitSignal(SignalName.StateTransition, this, "WalkState");
            }
        }
    }

    //public override void Exit() { //GD.Print("Exit WallSlideRight"); }
}