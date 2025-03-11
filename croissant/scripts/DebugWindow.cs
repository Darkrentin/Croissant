using Godot;
using System;

public partial class DebugWindow : FloatWindow
{
    // Called when the node enters the scene tree for the first time.
    [Export] public DialogueWindow dialogueWindow;
    public bool open = false;
    public override void _Ready()
    {
        base._Ready();
        Size = new Vector2I(1, 1);
        Position = GameManager.GetScreenPosition(0.26f, 0.46f);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
        if(Input.IsActionJustPressed("debug"))
        {
            if(!open)
            {
                GD.Print("Opening");
               	//dialogueWindow.ShowDialogueBox();
                //dialogueWindow.label.Text = "[wave amplitude=20]Hello World! [b]Hello World![/b] Hello World!Hello World!Hello World![rainbow] Hello World! [/rainbow][/wave]";

                StartExponentialResize(GameManager.GetScreenSize(0.24f, 0.49f), 0.5f);
                StartExponentialTransition(GameManager.GetScreenPosition(0.17f, 0.47f), 5f,Smoothness, true);
                open = true;
            }
            else{
                GD.Print("Closing");
                StartExponentialResize(GameManager.GetScreenSize(0.10f, 0.12f), 0.5f);
                StartExponentialTransition(GameManager.GetScreenPosition(0.70f, 0.41f), 5f,Smoothness,true);
                open = false;
            }
        }
		if(Input.IsActionJustPressed("debug2"))
		{
			GD.Print(Lib.GetCursorPosition()/GameManager.ScreenSize);
		}
    }
}
