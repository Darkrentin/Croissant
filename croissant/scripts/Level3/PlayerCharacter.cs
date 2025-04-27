using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public const float Speed = 1000.0f;
    public const float JumpVelocity = -1000.0f;
    public const float GravityBase = 2500.0f;
    public const float GravityExponentStart = 1.0f;  
    public const float GravityExponentFactor = 0.50f; 

    private float gravityAcceleration = GravityExponentStart;

    public override void _Ready()
    {
        AddToGroup("player");
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

        if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
        {
            velocity.Y = JumpVelocity;
        }

        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
        if (direction != Vector2.Zero)
        {
            velocity.X = direction.X * Speed;
        }
        else
        {
            velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
        }

        Velocity = velocity;
        MoveAndSlide();
    }
}
