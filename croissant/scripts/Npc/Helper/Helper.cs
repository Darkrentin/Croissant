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
				CursorWindow cursorWindow = States.CursorWindowScene.Instantiate<CursorWindow>();
				GameManager.GameRoot.AddChild(cursorWindow);
				Level2.CursorWindow = cursorWindow;
				Dialogue.StartDialogue(NpcName, "HelperTuto");
				break;
			case "HelperTuto":
				GameManager.virus.ShowNpc(GameManager.virus.RightDown);
				GameManager.virus.DialogueToPlayAfterTransition = "Virus/Helper Dialogue 1";
				break;
			case "Virus/Helper Dialogue 2":
				HideNpc(3);
				GameManager.virus.Dialogue.StartDialogue("Virus", "Virus/Helper Dialogue 3");
				break;
			case "EndLevel2":
				GetTree().Quit();
				break;
		}
	}

	public override void TransitionFinished()
	{
		base.TransitionFinished();
	}
}