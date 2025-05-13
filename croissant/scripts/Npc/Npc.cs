using Godot;
using System;

public abstract partial class Npc : FloatWindow
{
    [Export] public string NpcName;
    [Export] public AnimationTree AnimationTree;
    [Export] public DialogueWindow Dialogue { get; set; }
    public bool ForceDialoguePlacement = false;
    public Vector2I LeftUp = Vector2I.Zero;
    public Vector2I LeftDown;
    public Vector2I RightUp;
    public Vector2I RightDown;
    public string DialogueToPlayAfterTransition = "";
    public AnimationNodeStateMachinePlayback AnimationScreen;

    public virtual void Skip()
    {
        if (Dialogue.isDialogue && !Dialogue.LockSkip)
            Dialogue.NextLine();
    }

    public abstract void DialogueFinished(string name);

    public override void _Ready()
    {
        base._Ready();
        InitNpc();
        Visible = false;
    }

    public virtual void InitNpc()
    {
        Lib.Print($"Npc : {NpcName} initialized DialogueId :{DialogueWindow.Dialogueid}");
        AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
        Dialogue.Position = -Dialogue.Size*2;
        ForceDialoguePlacement = true;

        LeftDown = new Vector2I(0, GameManager.ScreenSize.Y - Size.Y);
        RightUp = new Vector2I(GameManager.ScreenSize.X - Size.X, 0);
        RightDown = new Vector2I(GameManager.ScreenSize.X - Size.X, GameManager.ScreenSize.Y - Size.Y);
        Title = NpcName;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("LeftClick"))
            Skip();
        
    }

    public void HideNpc(int side = -1)
    {
        Vector2I HidePosition = Lib.GetRandomPositionOutsideScreen(side, Math.Max(Size.X, Size.Y) * 2);
        ForceDialoguePlacement = true;
        if (!OnScreen())
            Position = HidePosition;
        else
            StartLinearTransition(HidePosition, 0.5f, reset: true);
        Dialogue.Visible = false;
        Dialogue.label.Text = "";
        Visible = false;
    }

    public void ShowNpc(Vector2I Position)
    {
        ForceDialoguePlacement = false;
        StartLinearTransition(Position, 0.1f, reset: true);
        GrabFocus();
        Visible = true;
    }

    public override void TransitionFinished()
    {
        base.TransitionFinished();
        if (DialogueToPlayAfterTransition != "")
        {
            Dialogue.StartDialogue(NpcName, DialogueToPlayAfterTransition);
            DialogueToPlayAfterTransition = "";
        }
    }

    public bool OnScreen()
    {
        return Position.X >= 0 && Position.Y >= 0 && Position.X <= GameManager.ScreenSize.X && Position.Y <= GameManager.ScreenSize.Y;
    }
}