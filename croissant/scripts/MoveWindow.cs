using Godot;
using System;

public partial class MoveWindow : PopUpWindow
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Parent = GetParent<Level1>();
		Parent.WindowCount++;
		Size = new Vector2I(180,70);
		SetWindowPosition(GameManager.GetScreenPosition(Lib.GetRandomNormal(0.1f,0.9f),Lib.GetRandomNormal(0.1f,0.9f)));
		StartNewMovement();
	}

	public override void OnClose()
	{
		Parent.WindowKillCount++;
		Parent.WindowCount--;
		GD.Print("cc");
		QueueFree();
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		base._Process(delta);
	}

    protected override void Update()
    {
        throw new NotImplementedException();
    }

    public override void TransitionFinished()
    {
        base.TransitionFinished();
		StartNewMovement();
    }

	public void StartNewMovement()
	{
		Vector2I target = GameManager.GetScreenPosition(Lib.GetRandomNormal(0.1f,0.9f),Lib.GetRandomNormal(0.1f,0.9f));
		StartExponentialTransition(target, Lib.rand.Next(5,50)/10f);
	}

}
