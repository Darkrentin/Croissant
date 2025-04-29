using Godot;

public partial class TankWindow : PopUpWindow
{
    [Export] public TextureRect Image;
    public Timer AlternateTimer = new Timer();
    public int state;
    public int HPs = 3;

    public override void _Ready()
    {
        HasChangingTitle = false;
        Size = Lib.GetAspectFactor(new Vector2I(560, 420));
        base._Ready();
        Title = "☻   ☻   ☻";

        AddChild(AlternateTimer);
        AlternateTimer.WaitTime = 0.5f;
        AlternateTimer.Timeout += OnAlternateTimerTimeout;
        AlternateTimer.Start();
    }

    public override void OnClose()
    {
        if (HPs == 1)
        {
            Level1.WindowKill();
            QueueFree();
        }
        else
        {
            HPs--;
            Title = "";
            for (int i = 0; i < HPs; i++)
                Title += "☻   ";
        }

        Image.Texture = GD.Load<Texture2D>($"res://assets/sprites/popups/yaai_{HPs}_{state}.png");
        StartShake(0.15f, 10);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void OnAlternateTimerTimeout()
    {
        state = state == 0 ? 1 : 0;
        Image.Texture = GD.Load<Texture2D>($"res://assets/sprites/popups/yaai_{HPs}_{state}.png");
    }
}