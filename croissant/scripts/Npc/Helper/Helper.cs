public partial class Helper : Npc
{
	public override void _Ready()
	{
		base._Ready();
		//Dialogue.StartDialogue(NpcName, "Restart");
	}

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