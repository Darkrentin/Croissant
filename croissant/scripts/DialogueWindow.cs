using Godot;
using System;
using System.Numerics;

public partial class DialogueWindow : FloatWindow
{
    // Called when the node enters the scene tree for the first time.

    [Export] public RichTextLabel label;
    [Export] public NinePatchRect background;
    [Export] public Timer timer;

    [Export] public FloatWindow Parent;

    public override void _Ready()
    {
        base._Ready();
        if(Parent == null)
        {
            Parent = GetParent() as FloatWindow;
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void SetDialogueBoxSize(int y)
    {
    
    }

    public void SetDialogueBoxPosition()
    {
        int x, y;
        
        
        x = (int)(Parent.Position.X - (Size.X/4));
        y = (int)(Parent.Position.Y + Parent.Size.Y - (Size.Y/2));
        
       
        x = Mathf.Clamp(x, 0, GameManager.ScreenSize.X - Size.X);
        y = Mathf.Clamp(y, 0, GameManager.ScreenSize.Y - Size.Y);
        
        Position = new Vector2I(x, y);
    }

    public void ShowDialogueBox()
    {
        Visible = true;
        
        SetDialogueBoxSize((int)(GameManager.ScreenSize.Y * 0.046f));
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
