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
		Position = new Vector2I(500, 500);
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

				StartExponentialResize(new Vector2I(457, 531), 0.5f);
				//StartLinearTransition(new Vector2I(532, 509), 0.5f);
				open = true;
			}
			else{
				GD.Print("Closing");
				StartExponentialResize(new Vector2I(199, 131), 0.5f);
				//StartLinearTransition(new Vector2I(1041, 144), 0.5f);
				open = false;
			}
		}
	}
}
