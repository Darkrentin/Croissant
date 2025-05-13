using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public const float Speed = 400.0f;
    public const float JumpVelocity = -950.0f;
    public const float GravityBase = 4000.0f; 
    public const float GravityExponentStart = 1.0f;
    public const float GravityExponentFactor = 0.25f;
    [Export] public AnimationPlayer AnimationPlayer;
    [Export] public Sprite2D Sprite;

    private float gravityAcceleration = GravityExponentStart;

    [Export] public Area2D area2D;

    private Timer wallJumpTimer;
    private bool isWallJumping = false;

    public override void _Ready()
    {
        AddToGroup("player");
        area2D.BodyEntered += OnBodyEntered;


        wallJumpTimer = new Timer();
        wallJumpTimer.WaitTime = 0.3f;
        wallJumpTimer.OneShot = true;
        AddChild(wallJumpTimer);
        wallJumpTimer.Timeout += OnWallJumpTimerTimeout;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is StaticPlatform platform)
        {
            //GD.Print("static test");
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 velocity = Velocity;

        if (!IsOnFloor())
        {
            gravityAcceleration += GravityExponentFactor * (float)delta;
            velocity.Y += GravityBase * gravityAcceleration * (float)delta;
        }
        else
        {
            gravityAcceleration = GravityExponentStart;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private void OnWallJumpTimerTimeout()
    {
        isWallJumping = false;
    }

    public void StartWallJumpTimer()
    {
        isWallJumping = true;
        wallJumpTimer.Start();
    }

    public bool IsWallJumping()
    {
        return isWallJumping;
    }

}
