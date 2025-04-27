using Godot;
using System;

public partial class Helper : Npc
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		//Dialogue.StartDialogue(NpcName, "Restart");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

	public override void DialogueFinished(string name)
	{
		// Implement the logic for when the dialogue is finished
		// For example, you can call a method to start a new dialogue or perform some action
		// Example: StartNewDialogue();
		switch (name)
		{
			case "Restart":
				HideNpc();
				GameManager.State = GameManager.GameState.Level2;
				break;
		}
	}

    public override void TransitionFinished()
    {
        base.TransitionFinished();
    }

}
