using Godot;
using System;
using System.Numerics;

public partial class DialogueWindow : FloatWindow
{
    // Called when the node enters the scene tree for the first time.

    [Export] public RichTextLabel label;
    [Export] public NinePatchRect background;
    [Export] public Timer timer;

    [Export] public Window Parent;

    [Export] public Button SkipButton;

    public override void _Ready()
    {
        base._Ready();
        if(Parent == null)
        {
            Parent = GetParent() as FloatWindow;
        }

        label.Theme = new Theme();
        label.Theme.DefaultFontSize = Lib.GetScreenSize(0.01f,0).X;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("debug"))
        {
            ShowDialogueBox();
        }
        
    }

    public void SetDialogueBoxSize()
    {
        Size = Lib.GetScreenSize(0.2f,0.1f);
        background.Size = Size;
        label.Size = new Vector2I((int)(Size.X*0.96f),(int)(Size.Y*0.7f));
        label.Position = ((Godot.Vector2)Size)*0.02f;
        Size = Lib.GetScreenSize(0.2f,0.12f);
        SkipButton.Size = Size*new Godot.Vector2(0.3f,0.2f);
    }

    public void SetDialogueBoxPosition()
    {
        int x, y;
        
        
        x = (int)(Parent.Position.X - (Size.X/4));
        y = (int)(Parent.Position.Y + Parent.Size.Y - (Size.Y/2));
        
       
        x = Mathf.Clamp(x, 0, GameManager.ScreenSize.X - Size.X);
        y = Mathf.Clamp(y, 0, GameManager.ScreenSize.Y - Size.Y);
        
        Position = new Vector2I(x, y);
        SkipButton.Position = Size*new Godot.Vector2(0.6f,0.7f);
    }

    public void ShowDialogueBox()
    {
        Visible = true;
        GrabFocus();
        SetDialogueBoxSize();
        SetDialogueBoxPosition();
        label.VisibleCharacters = 0;
        timer.Start();
    }

    public void _on_timer_timeout()
    {
        if(label.VisibleCharacters < label.GetTotalCharacterCount())
        {
            label.VisibleCharacters += 1;
            timer.Start();
        }
        else
        {
            timer.Stop();
        }
    }
}
