using Godot;
using System;

public partial class DialogueWindow : FloatWindow
{
	// Called when the node enters the scene tree for the first time.

	[Export] public Label label;
	[Export] public ColorRect background;

	[Export] public FloatWindow Parent;

	public override void _Ready()
	{
		if(Parent == null)
		{
			Parent = GetParent() as FloatWindow;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void SetDialogueBoxSize(int y)
	{
		Size = new Vector2I(Parent.Size.X, y);
		background.Size = Size;
		label.Size = Size;
	}

	public void SetDialogueBoxPosition()
	{
		int x,y;
		x = (int)(Parent.Position.X - (Size.X/4));
		y = (int)(Parent.Position.Y + Parent.Size.Y - (Size.Y/2));
		Position = new Vector2I(x, y);

	}

	public void ShowDialogueBox()
	{
		Visible = true;
		SetDialogueBoxSize(50);
		SetDialogueBoxPosition();
	}
}
