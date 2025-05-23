using Godot;

public partial class Helper : Npc
{
	[Export] public AnimationPlayer animationPlayer;
	[Export] public AnimatedSprite2D Sprite2D;

	public Vector2 BaseScale = new Vector2(10f, 10f);
	public Vector2I BaseSize = new Vector2I(300, 300);
	public Vector2 NewScale = new Vector2(1f, 1f);
	public override void _Ready()
	{
		base._Ready();
		Lib.Print($"Npc : {NpcName} initialized DialogueId :{Size}");
		//Dialogue.StartDialogue(NpcName, "Restart");
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
		if (IsResizing)
		{
			NewScale = new Vector2(BaseScale.X * Size.X / BaseSize.X, BaseScale.Y * Size.Y / BaseSize.Y);
			Sprite2D.Scale = NewScale;
		}
	}

	public override void ShowNpc(Vector2I Position, float time = 0.5f)
	{
		Size = BaseSize;
		Sprite2D.Scale = BaseScale;
		base.ShowNpc(Position,time);
		animationPlayer.Play("Spawn");
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
				GameManager.virus.ShowNpc(GameManager.virus.RightDown, 0.3f);
				GameManager.virus.DialogueToPlayAfterTransition = "Virus/Helper Dialogue 1";
				break;
			case "Virus/Helper Dialogue 2":
				HideNpc(3);
				GameManager.virus.Dialogue.StartDialogue("Virus", "Virus/Helper Dialogue 3");
				break;
			case "EndLvl2":
				GameManager.State = GameManager.GameState.Level3;
				break;
			case "EndLvl3":
				GameManager.State = GameManager.GameState.FinalLevel;
				break;
		}
	}

	public override void TransitionFinished()
	{
		base.TransitionFinished();
		if (TransitionTag == "Level3Spawn")
		{
			TransitionTag = "";
			Level3.Instance.ShowPlayer();

			Size = BaseSize;
			Lib.Print("Size: " + Size);
			Sprite2D.Scale = BaseScale;
			Position = -GameManager.ScreenSize / 2;
		}
	}
}
