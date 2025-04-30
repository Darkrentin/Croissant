using Godot;

public partial class PopUpWindow : FloatWindow
{
    protected bool HasChangingTitle = false;
    protected Timer TitleTimer;

    public override void _Ready()
    {
        base._Ready();
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f, 0.9f), Lib.GetRandomNormal(0.1f, 0.9f)));
        if (HasChangingTitle)
        {
            TitleTimer = new Timer
            {
                OneShot = false,
                WaitTime = Mathf.Max(0.1f, Lib.GetRandomNormal(0f, 0.5f)) // Ensure minimum wait time
            };
            AddChild(TitleTimer);
            TitleTimer.Timeout += ChangeTitle;
            TitleTimer.Start();
            ChangeTitle();
        }
    }

    protected virtual void ChangeTitle()
    {
        if (!HasChangingTitle)
            return;
        Title = Lib.GetCursedString();
        TitleTimer.WaitTime = Mathf.Max(0.1f, Lib.GetRandomNormal(0f, 0.5f));
        TitleTimer.Start();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        if (TitleTimer != null)
        {
            TitleTimer.Stop();
            TitleTimer.Timeout -= ChangeTitle;
        }
    }
}