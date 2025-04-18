using Godot;
using System;
using System.Numerics;

public partial class BombWindow : PopUpWindow
{
    [Export] public Timer timer;
    [Export] private ProgressBar progressBar;
    [Export] private AnimatedSprite2D Sprite;
    [Export] private TextureRect Texture;
    private float time;

    public override void _Ready()
    {
        HasChangingTitle = true;
        base._Ready();
        timer.Timeout += _on_timer_timeout;

        int randNum = Lib.rand.Next(0, 4);
        switch (randNum)
        {
            case 0:
                Texture.Texture = ResourceLoader.Load<Texture2D>("res://assets/popups/bomb1.png");
                break;
            case 1:
                Texture.Texture = ResourceLoader.Load<Texture2D>("res://assets/popups/bomb2.png");
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
        time = 8f;
        progressBar.MaxValue = time * 100f;
        timer.WaitTime = time;
        Size = (Vector2I)Lib.GetAspectFactor(new Vector2I(250, 250));
        progressBar.SetDeferred("size", Size);
        timer.Start();
    }

    public override void OnClose()
    {
        Level1.WindowCount--;
        for (int i = 0; i < 3; i++)
        {
            Level1.AddNewWindow();
        }
        QueueFree();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        progressBar.Value = timer.TimeLeft * 100f;
    }

    public void _on_timer_timeout()
    {
        Level1.WindowCount--;
        QueueFree();
    }
}
