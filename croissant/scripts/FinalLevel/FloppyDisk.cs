using Godot;
using System;
using System.Collections.Generic;

public partial class FloppyDisk : CharacterBody3D
{
    [Export]
    public float Speed { get; set; } = 6f;
    [Export] public Area3D Area;
    [Export] public AnimationPlayer AnimationPlayer;

    private Vector3 _direction = Vector3.Zero;
    private bool _isMoving = false;
    private float _timeToLive = 0f;
    private float _currentTime = 0f;

    public static List<FloppyDisk> FloppyDisks = new List<FloppyDisk>();

    public override void _Ready()
    {
        FloppyDisks.Add(this);
        Area.BodyEntered += OnBodyEntered;

        GetTree().CreateTimer(0.1f).Timeout += () =>
        {
            StartMovement(5f);
        };
    }

    public static void ResetFloppyDisks()
    {
        foreach (FloppyDisk floppyDisk in FloppyDisks)
        {
            floppyDisk.QueueFree();
        }
        FloppyDisks.Clear();
    }

    public void StartMovement(float duration)
    {
        // Set direction towards the player
        if (FinalLevel.Instance?.Player3D != null)
        {
            Vector3 playerPosition = FinalLevel.Instance.Player3D.GlobalPosition;
            Vector3 directionToPlayer = playerPosition - GlobalPosition;
            directionToPlayer.Y = 0; // Keep movement on the horizontal plane
            _direction = directionToPlayer.Normalized();
        }
        else
        {
            // Fallback: Set direction away from the center (0,0,0)
            Vector3 directionFromCenter = -GlobalPosition;
            directionFromCenter.Y = 0; // Keep movement on the horizontal plane
            _direction = directionFromCenter.Normalized();
        }

        _isMoving = true;
        _timeToLive = duration;
        _currentTime = 0f;

        // Look in the direction of movement
        if (_direction.LengthSquared() > 0.001f)
        {
            LookAt(GlobalPosition + _direction, Vector3.Up);
            GlobalRotation *= new Vector3(0, 1, 0);
            GlobalRotation += new Vector3(0, (float)Math.PI, 0);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!_isMoving)
            return;

        _currentTime += (float)delta;

        if (_currentTime >= _timeToLive)
        {
            Kill();
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

            ResetFloppyDisks();
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
            Kill();
        }
    }

    public void Kill()
    {
        FloppyDisks.Remove(this);
        QueueFree();
    }
}
