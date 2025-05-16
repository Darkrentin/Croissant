using Godot;

public partial class BombWindow : PopUpWindow
{
    [Export] private ProgressBar progressBar;
    [Export] private AnimatedSprite2D Sprite;
    [Export] private TextureRect Texture;
    [Export] public Timer timer;
    private float time;

    public override void _Ready()
    {
        HasChangingTitle = true;
        Size = Lib.GetAspectFactor(new Vector2I(250, 250));
        base._Ready();
        timer.Timeout += _on_timer_timeout;

        int randNum = Lib.rand.Next(0, 4);
        switch (randNum)
        {
            case 0:
                Texture.Texture = ResourceLoader.Load<Texture2D>("res://assets/sprites/popups/bomb1.png");
                break;
            case 1:
                Texture.Texture = ResourceLoader.Load<Texture2D>("res://assets/sprites/popups/bomb2.png");
                break;
            case 2:
                Sprite.Visible = true;
                Sprite.Animation = "banana";
                Sprite.Play();
                break;
            case 3:
                Sprite.Visible = true;
                Sprite.Animation = "spin";
                Sprite.Play();
                break;
        }
        time = 6f;
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;

        progressBar.SetDeferred("size", Size);
        timer.Start();
    }

    public override void OnClose()
    {
        Level1.WindowKill();
        for (int i = 0; i < 3; i++)
            Level1.AddNewWindow();
        QueueFree();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        progressBar.Value = timer.TimeLeft * 100f;
    }

    public void _on_timer_timeout()
    {
        Level1.WindowKill();
        QueueFree();
    }
}
