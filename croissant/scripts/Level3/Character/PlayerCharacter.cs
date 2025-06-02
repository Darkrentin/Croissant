using Godot;
using System;

public partial class PlayerCharacter : CharacterBody2D
{
    public const float ScaleFactor = 4f;

    [Export] public RigidBody2D Head;
    [Export] public Sprite2D Sprite;
    [Export] public AnimationPlayer Animator;
    [Export] public AnimationPlayer Animator2;
    [Export] public CollisionShape2D Collider;
    [Export] public Node States;
    [Export] public Timer CoyoteTimer;
    [Export] public Timer JumpBufferTimer;
    [Export] public Area2D area2D;
    [Export] public PackedScene JumpParticlesScene;
    [Export] public CpuParticles2D WalkParticles;

    [Export] public AudioStreamPlayer FootstepSound;
    [Export] public AudioStreamPlayer JumpSound;
    [Export] public AudioStreamPlayer LandingSound;
    [Export] public AudioStreamPlayer DeathSound;

    public Timer StepTimer;

    public bool FlipLock { get; set; } = false;

    public const float RunSpeed = 140f * ScaleFactor;
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
    public static float GravityWallSlide = 20f;
    public const float DirectionBufferTime = 0.2f;
    private int _lastDirPressTime = -1;
    private int _lastDirPressMsec = -1;

    public float moveSpeed = RunSpeed;
    public float Acceleration = GroundAcceleration;
    public float Deceleration = GroundDeceleration;
    public float jumpSpeed = JumpVelocity;
    public int moveDirectionX = 0;
    public int jumps = 0;
    public Vector2 wallDirection = Vector2.Zero;
    public int facing = 1;

    public Vector2 WallDirection { get; set; }

    public bool keyUp = false;
    public bool keyDown = false;
    public bool keyLeft = false;
    public bool keyRight = false;
    public bool keyJump = false;
    public bool isDead = false;
    public bool isInvincible = false;
    public bool LevelEnd = false;
    public bool keyJumpPressed = false;

    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    public Node currentState = null;
    public Node previousState = null;

    public bool _hasJumped = false;
    public int _fallEnterMsec = -1;

    public double TimeSinceFall => (_fallEnterMsec < 0)
        ? double.MaxValue
        : (((int)Time.GetTicksMsec() - _fallEnterMsec) / 1000.0);

    public override void _Ready()
    {
        Head.Sleeping = true;
        Head.Visible = false;
        Head.Freeze = true;
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

        ProcessMode = ProcessModeEnum.Disabled;
        Visible = false;
        Animator.AnimationFinished += OnAnimationFinished;

        StepTimer = new Timer();
        StepTimer.WaitTime = 0.26f;
        StepTimer.OneShot = false;
        StepTimer.Timeout += () =>
        {
            if (IsOnFloor())
                FootstepSound.Play();
        };
        AddChild(StepTimer);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        if (isDead) return;
        if (Input.IsActionJustPressed("ui_left") || Input.IsActionJustPressed("ui_right")) _lastDirPressMsec = (int)Time.GetTicksMsec();
        GetInputStates();
        currentState.Call("Update", delta);
        HandleMaxFallVelocity();
        MoveAndSlide();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("ui_left"))
        {
            isLeftPressed = @event.IsPressed();
            if (isLeftPressed)
                isRightPressed = false;
        }
        if (@event.IsAction("ui_right"))
        {
            isRightPressed = @event.IsPressed();
            if (isRightPressed)
                isLeftPressed = false;
        }
    }

    public void OnBodyEntered(Node body) { }

    public void HorizontalMovement(float acceleration = -1f, float deceleration = -1f)
    {
        if (acceleration < 0) acceleration = Acceleration;
        if (deceleration < 0) deceleration = Deceleration;

        moveDirectionX = 0;
        if (isLeftPressed) moveDirectionX -= 1;
        if (isRightPressed) moveDirectionX += 1;

        if (moveDirectionX != 0)
        {
            //GD.Print($"HorizontalMovement: moveDirectionX: {moveDirectionX}");
            Velocity = Velocity.MoveToward(new Vector2(moveDirectionX * moveSpeed, Velocity.Y), acceleration);
        }
        else
        {
            //GD.Print($"HorizontalMovement: moveDirectionX: {moveDirectionX} - Decelerating");
            Velocity = Velocity.MoveToward(new Vector2(0, Velocity.Y), deceleration);
        }
    }

    public void HandleFalling()
    {
        if (!IsOnFloor())
        {
            CoyoteTimer.Start(CoyoteTime);
            ChangeState((Node)States.Get("Fall"));
        }
        else if (LevelEnd)
        {
            isDead = true;
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
        // Appliquer la gravité réduite pour le wall slide
        Velocity += new Vector2(0, gravity / 2.0f * (float)delta);

        // Limiter la vitesse de chute
        if (Velocity.Y > MaxFallWallVelocity)
            Velocity = new Vector2(Velocity.X, MaxFallWallVelocity);

        // Réduire la vitesse de montée pour un wall slide plus réaliste
        if (Velocity.Y < 0) // Si on monte encore
        {
            // Appliquer une friction pour ralentir la montée
            float wallSlideFriction = 0.85f; // Facteur de friction (ajustez selon vos besoins)
            Velocity = new Vector2(Velocity.X, Velocity.Y * wallSlideFriction);
        }
    }

    public void HandleJump()
    {
        if (IsOnFloor())
        {
            if (jumps < MaxJumps && keyJumpPressed)
            {
                jumps++;
                StopWalkingEffects();
                ChangeState((Node)States.Get("Jump"));
            }
        }
        else
        {
            if (jumps < MaxJumps && jumps > 0 && keyJumpPressed)
            {
                jumps++;
                StopWalkingEffects();
                ChangeState((Node)States.Get("Jump"));
            }
            if (CoyoteTimer.TimeLeft > 0 && keyJumpPressed && jumps < MaxJumps)
            {
                CoyoteTimer.Stop();
                jumps++;
                StopWalkingEffects();
                ChangeState((Node)States.Get("Jump"));
            }
        }
    }

    public void HandleWallJump()
    {
        GetWallDirection();
        if ((keyJumpPressed && wallDirection.X != 0) || (keyLeft && wallDirection.X == 1) || (keyRight && wallDirection.X == -1))
        {
            StopWalkingEffects();
            ChangeState((Node)States.Get("WallJump"));
        }
    }

    public void StopWalkingEffects()
    {
        WalkParticles.Emitting = false;
        StepTimer.Stop();
    }

    public void StartWalkingEffects()
    {
        WalkParticles.Emitting = true;
        if (StepTimer.IsStopped())
        {
            StepTimer.Start();
        }
    }

    public void HandleLanding()
    {
        if (IsOnFloor())
        {
            jumps = 0;
            SpawnJumpParticles(Vector2.Up);
            WalkParticles.Emitting = false;
            LandingSound.Play();

            if (JumpBufferTimer.TimeLeft > 0)
            {
                JumpBufferTimer.Stop();
                jumps++;
                StopWalkingEffects();
                ChangeState((Node)States.Get("Jump"));
                return;
            }
            if (keyJump && jumps < MaxJumps)
            {
                jumps++;
                StopWalkingEffects();
                ChangeState((Node)States.Get("Jump"));
                return;
            }

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
                    wallDirection = Vector2.Left;
                    break;
                }
                else if (normal.X < -0.5f)
                {
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
        keyLeft = isLeftPressed;
        keyRight = isRightPressed;
        keyJump = Input.IsActionPressed("ui_up");
        keyJumpPressed = Input.IsActionJustPressed("ui_up");

        if (keyLeft) facing = -1;
        if (keyRight) facing = 1;

        Sprite.FlipH = (facing < 0);
    }

    public void ChangeState(Node nextState)
    {
        previousState = currentState;
        currentState = nextState;
        previousState.Call("ExitState");
        currentState.Call("EnterState");
    }

    public void HandleFlipH()
    {
        Sprite.FlipH = facing < 1;
    }

    public void Death()
    {
        if (isDead) return;
        Lib.Print("PlayerCharacter: Death");
        Animator.Play("Death");
        DeathSound.Play();
        isDead = true;
        Lib.Print("PlayerCharacter: Death Animation");
    }

    public void OnAnimationFinished(StringName animationName)
    {
        if (animationName == "Death")
        {
            Animator2.Play("Hide");
            Visible = false;

            GetTree().CreateTimer(0.3f).Timeout += () =>
            {
                Lib.Print("PlayerCharacter: Death Animation Finished");
                int previousSceneId = Level3.Instance.PreviousLevel(Level3.Instance.sceneid);
                Level3.Instance.actualScene.HideSubLevel();
                Level3.Instance.sceneid = previousSceneId;
                Level3.Instance.Level3Nodes[previousSceneId].ShowSubLevel();
                Level3.Instance.actualScene = Level3.Instance.Level3Nodes[previousSceneId];
                PortalExit PortalExit = Level3.Instance.actualScene.portalExit;

                GetTree().CreateTimer(0.1f).Timeout += () =>
                {
                    PortalExit.Visible = true;
                    PortalExit.AnimationPlayer.Play("Open");
                    Level3.Instance.PortalExitSound.Play();
                    GlobalPosition = PortalExit.GlobalPosition;

                    GetTree().CreateTimer(0.1f).Timeout += () =>
                    {
                        Animator2.PlayBackwards("Hide");
                        isDead = false;
                        Visible = true;

                    };
                };
            };



            if (Velocity.Y > 0)
            {
                Velocity = new Vector2(Velocity.X, 0);
            }

            if (Level3.Instance.End)
            {
                Level3.Instance.EndLevel();
            }
        }
        if (animationName == "Repair")
        {
            GameManager.helper.Dialogue.StartDialogue(GameManager.helper.NpcName, "HelperDeath", GameManager.ScreenSize / 2 - GameManager.helper.Dialogue.Size / 2 + new Vector2I(0, GameManager.ScreenSize.Y / 4));
        }
    }

    public void SpawnJumpParticles(Vector2 direction)
    {
        CpuParticles2D particles = JumpParticlesScene.Instantiate() as CpuParticles2D;
        GetParent().AddChild(particles);
        particles.GlobalPosition = GlobalPosition;

        if (direction == Vector2.Up)
        {
            particles.Rotation = 0;
        }
        else if (direction == Vector2.Right)
        {
            particles.Rotation = Mathf.Pi / 2;
            particles.Position += new Vector2(-20, -20);
        }
        else if (direction == Vector2.Left)
        {
            particles.Rotation = -Mathf.Pi / 2;
            particles.Position += new Vector2(20, -20);
        }

        particles.Emitting = true;

        Timer timer = new Timer();
        AddChild(timer);
        timer.WaitTime = 2.0f;
        timer.OneShot = true;
        timer.Timeout += () =>
        {
            if (IsInstanceValid(particles))
                particles.QueueFree();
            timer.QueueFree();
        };
        timer.Start();
    }

    public void SpawnWallJumpParticles(Vector2 wallDirection)
    {
        Vector2 particleDirection = wallDirection == Vector2.Left ? Vector2.Right : Vector2.Left;
        SpawnJumpParticles(particleDirection);
    }

    public void HandleWallSlideParticles()
    {
        if (IsOnWall() && wallDirection != Vector2.Zero)
        {
            WalkParticles.Emitting = true;

            if (wallDirection == Vector2.Left)
            {
                WalkParticles.Position = new Vector2(-20, -80);
            }
            else if (wallDirection == Vector2.Right)
            {
                WalkParticles.Position = new Vector2(20, -80);
            }
        }
        else
        {
            WalkParticles.Emitting = false;
            WalkParticles.Position = Vector2.Zero;
        }
    }
}
