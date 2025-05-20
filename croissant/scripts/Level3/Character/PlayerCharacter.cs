using Godot;
using System;
using System.Diagnostics;

public partial class PlayerCharacter : CharacterBody2D
{
    // Scale factor
    public const float ScaleFactor = 4f;

    // Nodes
    [Export] public Sprite2D Sprite;
    [Export] public AnimationPlayer Animator;
    [Export] public CollisionShape2D Collider;
    [Export] public Node States;

    [Export] public Timer CoyoteTimer;
    [Export] public Timer JumpBufferTimer;

    [Export] public RayCast2D RCBottomLeft; // i removed them of the code because it had a lot of bugs
    [Export] public RayCast2D RCBottomRight;

    [Export] public Area2D area2D;

    public bool FlipLock { get; set; } = false;

    // Physics constants (scaled for larger character)
    public const float RunSpeed = 125f * ScaleFactor;
    public const float WallJumpHSpeed = 120f * ScaleFactor;
    public const float GroundAcceleration = 20f * ScaleFactor;
    public const float GroundDeceleration = 25f * ScaleFactor;
    public const float AirAcceleration = 15f * ScaleFactor;
    public const float AirDeceleration = 20f * ScaleFactor;
    public const float WallKickAcceleration = 4f * ScaleFactor;
    public const float WallJumpAcceleration = 5f * ScaleFactor;
    public const float WallJumpYSpeedPeak = 0f;
    public const float GravityJump = 600f * ScaleFactor;
    public const float GravityFall = 700f * ScaleFactor;
    public const float MaxFallVelocity = 300f * ScaleFactor;

    public const float MaxFallWallVelocity = 100f * ScaleFactor;
    public const float JumpVelocity = -240f * ScaleFactor;
    public float WallJumpVelocity = -190f * ScaleFactor;
    public const float VariableJumpMultiplier = 0.5f;
    public const int MaxJumps = 1;
    public const float CoyoteTime = 0.1f;
    public const float JumpBufferTime = 0.15f;
    public static float GravityWallSlide = 20f; // chute lente

    public const float DirectionBufferTime = 0.2f;
    private int _lastDirPressTime = -1;
    private int _lastDirPressMsec = -1;


    // Physics variables
    public float moveSpeed = RunSpeed;
    public float Acceleration = GroundAcceleration;
    public float Deceleration = GroundDeceleration;
    public float jumpSpeed = JumpVelocity;
    public int moveDirectionX = 0;
    public int jumps = 0;
    public Vector2 wallDirection = Vector2.Zero;
    public int facing = 1;

    public Vector2 WallDirection { get; set; }

    // Input variables
    public bool keyUp = false;
    public bool keyDown = false;
    public bool keyLeft = false;
    public bool keyRight = false;
    public bool keyJump = false;
    public bool isDead = false;
    public bool isInvincible = true;
    public bool keyJumpPressed = false;

    // State machine
    public Node currentState = null;
    public Node previousState = null;

    public bool _hasJumped = false;
    public int _fallEnterMsec = -1;
    
    public double TimeSinceFall => (_fallEnterMsec < 0)
        ? double.MaxValue
        : (((int)Time.GetTicksMsec() - _fallEnterMsec) / 1000.0);


    public override void _Ready()
    {

        AddToGroup("PlayerCharacter");
        foreach (Node state in States.GetChildren())
        {
            state.Set("States", States);
            state.Set("Player", this);
        }
        previousState = (Node)States.Get("Fall");
        currentState = (Node)States.Get("Fall");

        CoyoteTimer.OneShot = true;
        JumpBufferTimer.OneShot = true;

        area2D.BodyEntered += OnBodyEntered;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_right")) _lastDirPressMsec = (int)Time.GetTicksMsec();
        GetInputStates();
        currentState.Call("Update", delta);
        HandleMaxFallVelocity();
        MoveAndSlide();
        //($"Current State: {currentState.Name}");

        if (isDead) return;
    }

    public void OnBodyEntered(Node body)
    {

    }

    public void HorizontalMovement(float acceleration = -1f, float deceleration = -1f)
    {
        if (acceleration < 0) acceleration = Acceleration;
        if (deceleration < 0) deceleration = Deceleration;

        moveDirectionX = 0;
        if (Input.IsActionPressed("ui_left")) moveDirectionX -= 1;
        if (Input.IsActionPressed("ui_right")) moveDirectionX += 1;
        if (moveDirectionX != 0)
            Velocity = Velocity.MoveToward(new Vector2(moveDirectionX * moveSpeed, Velocity.Y), acceleration);
        else
            Velocity = Velocity.MoveToward(new Vector2(0, Velocity.Y), deceleration);
    }

    public void HandleFalling()
    {
        if (!IsOnFloor())
        {
            CoyoteTimer.Start(CoyoteTime);
            ChangeState((Node)States.Get("Fall"));
        }
    }

    public void HandleMaxFallVelocity()
    {
        if (Velocity.Y > MaxFallVelocity)
            Velocity = new Vector2(Velocity.X, MaxFallVelocity);
    }

    public void HandleJumpBuffer()
    {
        if (keyJumpPressed && CoyoteTimer.TimeLeft <= 0)
            JumpBufferTimer.Start(JumpBufferTime);
    }

    public void HandleGravity(double delta, float gravity = GravityJump)
    {
        Velocity += new Vector2(0, gravity * (float)delta);
    }

    public void HandleGravityWallFall(double delta, float gravity = GravityJump)
    {
        if (Velocity.Y > MaxFallWallVelocity)
            Velocity = new Vector2(Velocity.X, MaxFallWallVelocity);
        Velocity += new Vector2(0, gravity/2.0f * (float)delta);
    }

    public void HandleJump()
    {
        if (IsOnFloor())
        {
            if (jumps < MaxJumps)
            {
                if (keyJumpPressed)
                {
                    jumps++;
                    ChangeState((Node)States.Get("Jump"));
                }
            }
        }
        else
        {
            if (jumps < MaxJumps && jumps > 0 && keyJumpPressed)
            {
                jumps++;
                ChangeState((Node)States.Get("Jump"));
            }
            if (CoyoteTimer.TimeLeft > 0 && keyJumpPressed && jumps < MaxJumps)
            {
                CoyoteTimer.Stop();
                jumps++;
                ChangeState((Node)States.Get("Jump"));
            }
        }
    }

    public void HandleWallJump()
    {
        GetWallDirection();
        if ((keyJumpPressed && wallDirection.X != 0)||(keyLeft && wallDirection.X == 1) || (keyRight && wallDirection.X == -1))
        {
            //GD.Print("WallJump");
            ChangeState((Node)States.Get("WallJump"));
        }
    }

    public void HandleLanding()
    {
        if (IsOnFloor())
        {
            jumps = 0;
            ChangeState((Node)States.Get("Idle"));
        }
    }
    
    public bool IsDirectionBuffered()
    {
        if (_lastDirPressMsec < 0) return false;
        int elapsed = (int)Time.GetTicksMsec() - _lastDirPressMsec;
        return elapsed <= DirectionBufferTime * 1000;
    }

    public void GetWallDirection()
    {
        wallDirection = Vector2.Zero;
        for (int i = 0; i < GetSlideCollisionCount(); i++)
        {
            var collision = GetSlideCollision(i);
            if (collision != null)
            {
                Vector2 normal = collision.GetNormal();
                if (normal.X > 0.5f)
                {
                    //GD.Print("Mur à gauche");
                    wallDirection = Vector2.Left;
                    break; 
                }
                else if (normal.X < -0.5f) 
                {
                    //GD.Print("Mur à droite");
                    wallDirection = Vector2.Right;
                    break; 
                }
            }
        }
    }


        public void HandleFallAnimations()
        {
            if (IsOnWall())
            {
                Animator.Play("WallSlideRight");
            }
            else
            {
                Animator.Play("Fall");
                HandleFlipH();
            }

        }


    public void GetInputStates()
    {
        keyUp = Input.IsActionPressed("ui_up");
        keyDown = Input.IsActionPressed("ui_down");
        keyLeft = Input.IsActionPressed("ui_left");
        keyRight = Input.IsActionPressed("ui_right");
        keyJump = Input.IsActionPressed("ui_up");
        keyJumpPressed = Input.IsActionJustPressed("ui_up");

        if (keyLeft) facing = -1;
        if (keyRight) facing = 1;

        Sprite.FlipH = (facing < 0);
    }

    public void ChangeState(Node nextState)
    {
        if (nextState != null)
        {
            previousState = currentState;
            currentState = nextState;
            previousState.Call("ExitState");
            currentState.Call("EnterState");
        }
    }

    public void HandleFlipH()
    {
        Sprite.FlipH = (facing < 1);
    }
    

    public void Death()
    {
        Animator.Play("Death");
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

            Animator.Play("Idle");
            isDead = false;
            Vector2 playerTargetPosition = new Vector2(1920 / 2, 1080 / 2);

            Tween tween = GetTree().CreateTween();
            float distance = (GlobalPosition - playerTargetPosition).Length();
            float screenWidth = GameManager.ScreenSize.X;
            float duration = Math.Max(distance / screenWidth, 0.1f);
            tween.SetTrans(Tween.TransitionType.Sine);
            tween.SetEase(Tween.EaseType.InOut);
            tween.TweenProperty(this, "global_position", playerTargetPosition, duration);
            tween.TweenCallback(Callable.From(() =>
            {
                ProcessMode = ProcessModeEnum.Pausable;
            }));
            ProcessMode = ProcessModeEnum.Disabled;

            if (Velocity.Y > 0)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }
        }
    }
}
