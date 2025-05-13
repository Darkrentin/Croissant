using Godot;
using System;

public partial class WallSlideLeft : State
{
    [Export] public float WallSlideSpeed = 50.0f;
    [Export] public float PushOffForce = -450.0f;

    private float horizontalDirection = 0f;

    public override void Enter()
    {
        //GD.Print("WallSlideLeft");
        var fsm = GetParent();
        var player = fsm?.GetParent() as PlayerCharacter;
        if (player != null)
        {
            Vector2 velocity = player.Velocity;
            velocity.X = 0;
            player.Velocity = velocity;
        }
        
        player.Sprite.FlipH = true;
        player.AnimationPlayer.Play("WallSlideLeft");
    }

    public override void Update(float delta)
    {
        var player = GetParent().GetParent<PlayerCharacter>();
        if (player == null)
            return;

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_down", "ui_up");

        if (Input.IsActionPressed("ui_left"))
        {
            horizontalDirection = -1f;
        }
        else if (Input.IsActionPressed("ui_right"))
        {
            horizontalDirection = 1f;
        }
        else
        {
            horizontalDirection = 0f;
        }
        
        if (player.IsOnWall())
        {
            player.Velocity = new Vector2(player.Velocity.X, WallSlideSpeed);
        }
        else
        {
            EmitSignal(SignalName.StateTransition, this, "FallState");
            return; 
        }

        
        if (Input.IsActionJustPressed("ui_up"))
        {
            player.Position -= new Vector2(0.1f, 0);
            player.Velocity = new Vector2(PushOffForce, PlayerCharacter.JumpVelocity);
            player.StartWallJumpTimer();
            EmitSignal(SignalName.StateTransition, this, "JumpState");
            return;
        }

        if (Input.IsActionJustPressed("ui_left"))
        {
            player.Position -= new Vector2(0.1f, 0);
            player.Velocity = new Vector2(PushOffForce, horizontalDirection * PlayerCharacter.Speed);
            EmitSignal(SignalName.StateTransition, this, "FallState");
            return;
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

    //public override void Exit() { //GD.Print("Exit WallSlideLeft"); }
}
