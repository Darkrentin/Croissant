using Godot;
using System;

public partial class AttackWindow : FloatWindow
{

    public enum Phase
    {
        Move,
        Prevent,
        Attack,
        Reload
    }
    protected Level2 Parent;
    public Timer Timer;

    public Vector2I windowSize;
	public Vector2I windowPosition;

    public static PackedScene VisualCollisionScene = GD.Load<PackedScene>("uid://chuegp025s2xl");
    public ColorRect VisualCollision;

    private Phase _phase = Phase.Move; 
    public Phase CurrentPhase {
        get => _phase;
        set {
            _phase = value;
            Lib.Print($"Current phase: {value}");
        }
    }
    public override void _Ready()
    {
        base._Ready();
        Timer = new Timer();
        Timer.OneShot = true;
        AddChild(Timer);
        Parent = GetParent<Level2>();

        windowSize = Size;

        VisualCollision = VisualCollisionScene.Instantiate<ColorRect>();
        VisualCollision.Size = Size;
        VisualCollision.Visible = false;
        GameManager.GameRoot.AddChild(VisualCollision);

        Start();
    }

    public void ShowVisualCollision(Vector2I size, Vector2 position)
    {
        VisualCollision.Position = position;
        VisualCollision.Size = size;
        VisualCollision.Visible = true;
    }

    public void HideVisualCollision()
    {
        VisualCollision.Visible = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        /*
        if (CurrentPhase == Phase.Attack && !Shaking && IsCollided(Parent.CursorWindow))
		{
			Parent.CursorWindow.TakeDamage();
		}
        */
    }

    public virtual void Start()
    {
        Timer.Timeout += Move;
        Move();
    }

    public virtual void Move()
    {
        Timer.Timeout-=Move;
        Timer.Timeout+=Prevent;

        CurrentPhase = Phase.Move;
        Timer.Start();
    }

    public virtual void Prevent()
    {
        Timer.Timeout -= Prevent;
        Timer.Timeout += Attack;

        CurrentPhase = Phase.Prevent;
        Timer.Start();
    }

    public virtual void Attack()
    {
        Timer.Timeout -= Attack;
        Timer.Timeout += Reload;

        CurrentPhase = Phase.Attack;
        Timer.Start();
    }

    public virtual void Reload()
    {
        Timer.Timeout -= Reload;
        Timer.Timeout += Move;

        CurrentPhase = Phase.Reload;
        Timer.Start();
    }
}