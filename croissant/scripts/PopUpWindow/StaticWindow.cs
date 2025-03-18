using Godot;
using System;

public partial class StaticWindow : PopUpWindow
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		base._Ready();
		Parent = GetParent<Level1>();
		Size = new Vector2I(200,100);
		SetWindowPosition(Lib.GetScreenPosition(Lib.GetRandomNormal(0f,0.95f),Lib.GetRandomNormal(0f,0.95f)));
		
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

    protected override void Update()
    {
        throw new NotImplementedException();
    }

}
