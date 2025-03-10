using Godot;
using System;

public partial class DebugWindow : FloatWindow
{
	// Called when the node enters the scene tree for the first time.
	[Export] public DialogueWindow dialogueWindow;
	public override void _Ready()
	{
		dialogueWindow.ShowDialogueBox();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
