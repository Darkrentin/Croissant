using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public const float Speed = 1000.0f;
    public const float JumpVelocity = -1000.0f;
    public const float GravityBase = 2500.0f;
    public const float GravityExponentStart = 1.0f;
    public const float GravityExponentFactor = 0.50f;
    [Export] public AnimationPlayer AnimationPlayer;
    [Export] public Sprite2D Sprite;

    private float gravityAcceleration = GravityExponentStart;

    [Export] public Area2D area2D;

    public override void _Ready()
    {
        AddToGroup("player");
        area2D.BodyEntered += OnBodyEntered;
    }

    public void OnBodyEntered(Node body)
    {
        if (body is StaticPlatform platform)
        {
            GD.Print("static test");
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

}
