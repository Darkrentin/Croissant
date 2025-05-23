using Godot;
using System;

public abstract partial class Npc : FloatWindow
{
    [Export] public string NpcName;

    [Export] public DialogueWindow Dialogue { get; set; }
    [Export] public AnimationTree AnimationTree;
    public bool ForceDialoguePlacement = false;
    public Vector2I LeftUp = Vector2I.Zero;
    public Vector2I LeftDown;
    public Vector2I RightUp;
    public Vector2I RightDown;
    public string DialogueToPlayAfterTransition = "";
    public Timer HideTimer;
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
        HideTimer = new Timer();
        HideTimer.OneShot = true;
        HideTimer.Timeout += () => { Visible = false; };
        AddChild(HideTimer);
    }

    public virtual void InitNpc()
    {
        Lib.Print($"Npc : {NpcName} initialized DialogueId :{DialogueWindow.Dialogueid}");

        Dialogue.Position = -Dialogue.Size * 2;
        ForceDialoguePlacement = true;

        LeftDown = new Vector2I(0, GameManager.ScreenSize.Y - Size.Y);
        RightUp = new Vector2I(GameManager.ScreenSize.X - Size.X, 0);
        RightDown = new Vector2I(GameManager.ScreenSize.X - Size.X, GameManager.ScreenSize.Y - Size.Y);
        Title = NpcName;
        if(AnimationTree != null)
            AnimationScreen = (AnimationNodeStateMachinePlayback)(AnimationTree.Get("parameters/playback"));
    }

    public void PlayAnimation(string animationName)
    {
        if (AnimationTree != null)
        {
            AnimationScreen.Travel(animationName);
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Input.IsActionJustPressed("LeftClick"))
            Skip();
    }

    public virtual void HideNpc(int side = -1)
    {
        const float HideTime = 0.5f;
        Vector2I HidePosition = Lib.GetRandomPositionOutsideScreen(side, Math.Max(Size.X, Size.Y) * 2);
        ForceDialoguePlacement = true;
        if (!OnScreen())
        {
            Lib.Print($"Npc : {NpcName} is not on screen, hiding it");
            Position = HidePosition;
        }
        else
            StartLinearTransition(HidePosition, HideTime, reset: true);
        Dialogue.Visible = false;
        Dialogue.label.Text = "";
        HideTimer.Start(HideTime);
    }

    public virtual void ShowNpc(Vector2I Position, float time = 0.5f)
    {
        ForceDialoguePlacement = false;
        StartLinearTransition(Position, time, reset: true);
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
        bool IsOnScreen = Position.X >= 0 && Position.Y >= 0 && Position.X <= GameManager.ScreenSize.X && Position.Y <= GameManager.ScreenSize.Y;
        return IsOnScreen;
    }
}
