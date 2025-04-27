using Godot;

public partial class TankWindow : PopUpWindow
{
    [Export] public ProgressBar progressBar;
    public int HPs = 3;

    public override void _Ready()
    {
        HasChangingTitle = false;
        Size = Lib.GetAspectFactor(new Vector2I(437, 526));
        base._Ready();
        Title = "---";
        CheckHp();
    }

    public override void OnClose()
    {
        HPs--;
        if (HPs <= 0)
        {
            Level1.WindowKill();
            QueueFree();
        }
        else
            CheckHp();
        StartShake(0.15f, 10);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public void CheckHp()
    {
        Title = "";
        for (int i = 0; i < HPs; i++)
            Title += "❤︎";
    }
}