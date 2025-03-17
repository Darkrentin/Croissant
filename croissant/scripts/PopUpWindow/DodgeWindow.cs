using Godot;
using System;

public partial class DodgeWindow : PopUpWindow
{
    //this window will dodge the user mouse
	private bool shouldDodge = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Parent = GetParent<Level1>();
		Size = new Vector2I(200,200);
		SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0.1f,0.9f),Lib.GetRandomNormal(0.1f,0.9f)));
	}

	public override void OnClose()
	{
		Parent.WindowKillCount++;
		Parent.WindowCount--;
		QueueFree();
		
	}
	private bool IsMouseOnCloseButton()
	{
		
		Vector2 localMousePos = Lib.GetCursorPosition() - Position;
		
		float buttonLeftX = Size.X - 35;  
		float buttonRightX = Size.X;    
		float buttonTopY = -35;            
		float buttonBottomY = 0;        
		
		return localMousePos.X >= buttonLeftX && 
			localMousePos.X <= buttonRightX &&
			localMousePos.Y >= buttonTopY && 
			localMousePos.Y <= buttonBottomY;
	}

	
	public override void _Process(double delta)
	{
		base._Process(delta);
        //GD.Print($"Mouse on close: {IsMouseOnCloseButton()}, shouldDodge: {shouldDodge}");
        
        if (IsMouseOnCloseButton())
        {
            //shouldDodge = true;
            StartNewMovement();
        }
	}

    /*public override void TransitionFinished()
    {
        base.TransitionFinished();
        shouldDodge = false;
    }*/

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
        Vector2I target = Lib.GetScreenPosition(Lib.GetRandomNormal(0.1f, 0.9f), Lib.GetRandomNormal(0.1f, 0.9f));
        float speed = CalculateMovementSpeed();
        StartExponentialTransition(target, speed);
    }

	    protected override void Update()
    {
        throw new NotImplementedException();
    }

}
