using Godot;

public partial class AttackWindow : FloatWindow
{
    [Export] private PackedScene ExportVisualCollisionScene { get => VisualCollisionScene; set => VisualCollisionScene = value; }
    [Export] public bool RandomPosition = false;
    [Export] public bool Disable = false;
    private Phase _phase = Phase.Move;
    protected Level2 Parent;
    public enum Phase { Move, Prevent, Attack, Reload, Dammage }
    public Timer Timer;
    public Vector2I windowSize;
    public Vector2I windowPosition;
    public static PackedScene VisualCollisionScene;
    public VisualCollision VisualCollision;
    public int Lives = 3;
    public Phase CurrentPhase { get => _phase; set { _phase = value; } }
    public Vector2I CursorPosition
    {
        get
        {
            if (RandomPosition)
                return new Vector2I(Lib.rand.Next(0, GameManager.ScreenSize.X - Size.X), Lib.rand.Next(0, GameManager.ScreenSize.Y - Size.Y));
            else
                return Level2.CursorWindow.Position + Level2.CursorWindow.Size / 2;
        }
        set { Level2.CursorWindow.Position = value; }
    }

    public override void _Ready()
    {
        SharpCorners = true;
        Unresizable = true;
        Draggable = false;
        Minimizable = false;
        base._Ready();

        const int WindowSizeX = 160;
        int newSize = Lib.GetAspectFactor(new Vector2I(WindowSizeX, WindowSizeX)).X;
        Size = new Vector2I(newSize, newSize - TitleBarHeight);

        Position = Lib.GetRandomPositionOutsideScreen(-1, Size.X * 2);
        ////Lib.Print(TitleBarHeight.ToString());

        Timer = new Timer();
        Timer.OneShot = true;
        AddChild(Timer);

        windowSize = Size;

        VisualCollision = VisualCollisionScene.Instantiate<VisualCollision>();
        VisualCollision.Size = Size;
        VisualCollision.Visible = false;
        GameManager.GameRoot.AddChild(VisualCollision);

        Timer.WaitTime = Lib.GetRandomNormal(0.1f, 5.0f);
        Timer.Timeout += Start;

        if (!Disable)
        {
            Parent = Level2.Instance;
            Timer.Start();
        }
        else
        {
            Timer.Stop();
        }
    }

    public void ShowVisualCollision(Vector2I size, Vector2 position, float duration = 1f)
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
        if (CurrentPhase == Phase.Attack && !Shaking && IsCollided(Level2.CursorWindow))
        {
            Level2.CursorWindow.TakeDamage();
            CurrentPhase = Phase.Dammage;
        }
        if (CurrentPhase == Phase.Dammage && !IsCollided(Level2.CursorWindow))
            CurrentPhase = Phase.Attack;
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
        Lives--;
        Timer.Timeout -= Reload;
        if (Lives <= 0)
        {
            Delete();
            return;
        }
        Timer.Timeout += Move;

        CurrentPhase = Phase.Reload;
        Timer.Start();
    }

    public virtual void Delete()
    {
        if (VisualCollision != null && IsInstanceValid(VisualCollision) && VisualCollision.GetParent() != null)
        {
            GameManager.GameRoot.RemoveChild(VisualCollision);
            VisualCollision.QueueFree();
        }
        
        GrabFocus();
        Level2.CursorWindow.GrabFocus();

        Timer death = new Timer();
        AddChild(death);
        death.Timeout += QueueFree;
        Timer.WaitTime = 0.1f;
        Timer.OneShot = true;
        Hide();
        death.Start();
    
    }
}