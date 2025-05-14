using Godot;
using System;
using System.Diagnostics;

public partial class PlayerCharacter : CharacterBody2D
{
    public const float Speed = 400.0f;
    public const float JumpVelocity = -950.0f;
    public const float GravityBase = 4000.0f; 
    public const float GravityExponentStart = 1.0f;
    public const float GravityExponentFactor = 0.25f;
    [Export] public AnimationPlayer AnimationPlayer;
    [Export] public Sprite2D Sprite;
    public bool isDead = false;

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
        AnimationPlayer.AnimationFinished += OnAnimationFinished;
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
        if(isDead)
        {
            return;
        }
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

    public void Death()
    {
        AnimationPlayer.Play("Death");
        isDead = true;
    }

    public void OnAnimationFinished(StringName animationName)
    {
        if (animationName == "Death")
        {
            Level3.Instance.actualScene.HideSubLevel();
            Level3.Instance.sceneid = 0;
            Level3.Instance.Level3Nodes[0].ShowSubLevel();
            Level3.Instance.actualScene = Level3.Instance.Level3Nodes[0];
            
            AnimationPlayer.Play("Idle");
            isDead = false;
            Vector2 playerTargetPosition = new Vector2(1920/2, 1080/2);

            Tween tween = GetTree().CreateTween();
            float distance = (GlobalPosition - playerTargetPosition).Length();
            float screenWidth = GameManager.ScreenSize.X;
            float duration = Math.Max(distance / (float)screenWidth,0.1f); // Time to cross full screen = 1 second
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);
            tween.TweenProperty(this, "global_position", playerTargetPosition, duration);
            tween.TweenCallback(Callable.From(() => {
                ProcessMode = ProcessModeEnum.Pausable;
            }));
            ProcessMode = ProcessModeEnum.Disabled;
        }
    }   

}
