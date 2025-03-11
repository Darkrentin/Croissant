using Godot;
using System;

public partial class DebugWindow : FloatWindow
{
	// Called when the node enters the scene tree for the first time.
	[Export] public DialogueWindow dialogueWindow;
	public override void _Ready()
	{
		base._Ready();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
		if(Input.IsKeyPressed(Key.Enter))
		{
			dialogueWindow.ShowDialogueBox();
			dialogueWindow.label.Text = "[wave amplitude=20]Hello World! [b]Hello World![/b] Hello World!Hello World!Hello World![rainbow] Hello World! [/rainbow][/wave]";
		}
	}
}
