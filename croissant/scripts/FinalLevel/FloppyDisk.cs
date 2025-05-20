using Godot;
using System;

public partial class FloppyDisk : CharacterBody3D
{
    [Export]
    public float Speed { get; set; } = 5f;
    [Export] public Area3D Area;
    [Export] public AnimationPlayer AnimationPlayer;

    private Vector3 _direction = Vector3.Zero;
    private bool _isMoving = false;
    private float _timeToLive = 0f;
    private float _currentTime = 0f;

    [Export] public bool StartMovementButton { get => false; set { if (value) StartMovement(new Vector3(0, 0, 1), 10f); } }

    public override void _Ready()
    {
        Area.BodyEntered += OnBodyEntered;
        AnimationPlayer.AnimationFinished += OnAnimationFinished;
    }

    public void StartMovement(Vector3 direction, float duration)
    {
        _direction = direction.Normalized();
        _isMoving = true;
        _timeToLive = duration;
        _currentTime = 0f;

        LookAt(GlobalPosition + _direction, Vector3.Up);
        GlobalRotation *= new Vector3(0, 1, 0);
        GlobalRotation += new Vector3(0, (float)Math.PI, 0);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isMoving)
            return;

        _currentTime += (float)delta;

        if (_currentTime >= _timeToLive)
        {
            QueueFree();
            return;
        }

        Vector3 velocity = _direction * Speed;
        Velocity = velocity;
        MoveAndSlide();
    }
    public void OnBodyEntered(Node body)
    {
        if (body is Player3D player)
        {
            FinalLevel.Instance.DeathBossLevel();
            QueueFree();
        }
    }

    public void TakeDamage()
    {
        AnimationPlayer.Play("Death");
        Speed = 0f;
    }

    public void OnAnimationFinished(StringName animName)
    {
        if (animName == "Death")
        {
            QueueFree();
        }
    }
}
