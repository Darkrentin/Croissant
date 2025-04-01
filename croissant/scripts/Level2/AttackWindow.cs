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
    public static PackedScene VisualCollisionScene;
    [Export] private PackedScene ExportVisualCollisionScene { get => VisualCollisionScene; set => VisualCollisionScene = value; }
    public VisualCollision VisualCollision;
    private Phase _phase = Phase.Move;
    public Vector2I CursorPosition
    {
        get
        {
            if (Random)
                return new Vector2I(Lib.rand.Next(0, GameManager.ScreenSize.X - Size.X), Lib.rand.Next(0, GameManager.ScreenSize.Y - Size.Y));
            else
                return Parent.CursorWindow.Position + Parent.CursorWindow.Size / 2;
        }
        set
        {
            Parent.CursorWindow.Position = value;
        }
    }
    [Export] public bool Random = false;
    [Export] public bool Disable = false;
    public Phase CurrentPhase
    {
        get => _phase;
        set
        {
            _phase = value;
            Lib.Print($"Current phase: {value}");
        }
    }

    public override void _Ready()
    {
        SharpCorners = true;
        base._Ready();

        const int WindowSizeX = 120;
        Size = Lib.GetAspectFactor(new Vector2I(WindowSizeX, WindowSizeX));

        Lib.Print(TitleBarHeight.ToString());

        Timer = new Timer();
        Timer.OneShot = true;
        AddChild(Timer);

        windowSize = Size;

        VisualCollision = VisualCollisionScene.Instantiate<VisualCollision>();
        VisualCollision.Size = Size;
        VisualCollision.Visible = false;
        GameManager.GameRoot.AddChild(VisualCollision);

        Timer.WaitTime = Lib.GetRandomNormal(0.1f, 2.0f);
        Timer.Timeout += Start;

        if (!Disable)
        {
            Parent = GetParent<Level2>();
            Timer.Start();
        }
        else
        {
            GD.Print("Disabled");
            QueueFree();
        }
    }

    public void ShowVisualCollision(Vector2I size, Vector2 position, float duration = 0.5f)
    {
        VisualCollision.duration = duration;
        VisualCollision.elapsedTime = 0f;
        VisualCollision.Position = position - TitleBarSize;
        VisualCollision.Size = size + TitleBarSize;
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
        Timer.Timeout -= Start;
        Timer.Timeout += Move;
        Move();
    }

    public virtual void Move()
    {
        Timer.Timeout -= Move;
        Timer.Timeout += Prevent;

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
        CollisionDisabled = false;
        Timer.Timeout -= Attack;
        Timer.Timeout += Reload;

        CurrentPhase = Phase.Attack;
        Timer.Start();
    }

    public virtual void Reload()
    {
        CollisionDisabled = true;
        Timer.Timeout -= Reload;
        Timer.Timeout += Move;

        CurrentPhase = Phase.Reload;
        Timer.Start();
    }

    public override void WindowCollided(FloatWindow window)
    {
        if (window is CursorWindow w && CurrentPhase == Phase.Attack && !w.Shaking)
        {
            Lib.Print("Collided");
            w.TakeDamage();
        }
    }
}