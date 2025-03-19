using Godot;
using System;
using System.Reflection.PortableExecutable;

public partial class TankWindow: PopUpWindow
{

	[Export] public ProgressBar progressBar;
    private int clickcount = 0;
    private int maxClicks;
	public override void _Ready()
	{
        base._Ready();
        Parent = GetParent<Level1>();
        Size = new Vector2I(400,400);
        SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f,0.90f),Lib.GetRandomNormal(0f,0.90f)));
        maxClicks = Lib.rand.Next(2, 6);
    }

	public override void OnClose()
	{

        if (clickcount >= maxClicks)
        {
            Parent.WindowKillCount++;
            Parent.WindowCount--;
            QueueFree();
        }
        else
        {
            clickcount++;
        }
        GD.Print($"Clickcount: {clickcount}");
        StartShake(0.2f, 10);
	}

	public override void _Process(double delta)
	{
		base._Process(delta);
        Title = "";
        for (int i = 0; i <= maxClicks - clickcount; i++)
        {
            Title += "▯";
        }

	}

    protected override void Update()
    {
        throw new NotImplementedException();
    }





}
