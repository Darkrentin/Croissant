using Godot;
using System;
public abstract partial class Npc : FloatWindow
{
    [Export] public string NpcName;
    public bool ForceDialoguePlacement = false;

    public Vector2I LeftUp = Vector2I.Zero;
	public Vector2I LeftDown;
	public Vector2I RightUp;
	public Vector2I RightDown;

    public string DialogueToPlayAfterTransition = "";

	[Export] public AnimationTree AnimationTree;
	public AnimationNodeStateMachinePlayback AnimationScreen;

    [Export] public DialogueWindow Dialogue { get; set; }
    

    public virtual void Skip()
    {
        if (Dialogue.isDialogue)
			Dialogue.NextLine();
    }

    public abstract void DialogueFinished(string name);

    public override void _Ready()
    {
        base._Ready();
        InitNpc();
    }

    public virtual void InitNpc()
    {
        AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
        Dialogue.PlaceDialogueWindow();
        ForceDialoguePlacement = false;
		Dialogue.OnDialogueFinished += DialogueFinished;

        LeftDown = new Vector2I(0, GameManager.ScreenSize.Y - Size.Y);
		RightUp = new Vector2I(GameManager.ScreenSize.X - Size.X, 0);
		RightDown = new Vector2I(GameManager.ScreenSize.X - Size.X, GameManager.ScreenSize.Y - Size.Y);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("LeftClick"))
            Skip();
    }
    

    public void HideNpc(int side = -1)
	{
		Vector2I HidePosition = Lib.GetRandomPositionOutsideScreen(side,Math.Max(Size.X, Size.Y)*2);
		ForceDialoguePlacement = true;
		StartTransition(HidePosition, 0.5f,reset:true);
		Dialogue.Visible = false;
		Dialogue.label.Text = "";
	}

	public void ShowNpc(Vector2I Position)
	{
		ForceDialoguePlacement = false;
		StartTransition(Position, 0.1f,reset:true);
		GrabFocus();
	}

    public override void TransitionFinished()
    {
        base.TransitionFinished();
        if (DialogueToPlayAfterTransition != "")
		{
			GameManager.virus.Dialogue.StartDialogue(NpcName, DialogueToPlayAfterTransition);
			DialogueToPlayAfterTransition = "";
		}
    }
}