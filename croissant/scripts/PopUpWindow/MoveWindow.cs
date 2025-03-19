using Godot;
using System;

public partial class MoveWindow : PopUpWindow
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Parent = GetParent<Level1>();
		Size = new Vector2I(360,220);
		SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f,0.90f),Lib.GetRandomNormal(0f,0.90f)));
		StartNewMovement();
	}

	public override void OnClose()
	{
		Parent.WindowKillCount++;
		Parent.WindowCount--;
		QueueFree();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

    public override void TransitionFinished()
    {
        base.TransitionFinished();
		StartNewMovement();
    }

	    private float CalculateMovementSpeed()
    {
        if (Parent.WindowCount >= 10)
        {
            return Lib.rand.Next(80, 150) / 10f; 
        }
        else if (Parent.WindowCount >= 5)
        {
            return Lib.rand.Next(30, 50) / 10f; 
        }
        else
        {
            return Lib.rand.Next(15, 30) / 10f;
		}
    } 

    public void StartNewMovement()
    {
        Vector2I target = Lib.GetScreenPosition(Lib.GetRandomNormal(0.2f,0.80f),Lib.GetRandomNormal(0.2f,0.80f));
        float speed = CalculateMovementSpeed();
        StartExponentialTransition(target, speed,reset: true);
    }
}
