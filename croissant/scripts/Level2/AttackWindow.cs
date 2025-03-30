using Godot;
using System;

public partial class AttackWindow : FloatWindow
{
    protected Level2 Parent;
    public Timer Timer;
    public bool Attacking = false;

    public Vector2I windowSize;
	public Vector2I windowPosition;

    public static PackedScene VisualCollisionScene = GD.Load<PackedScene>("uid://chuegp025s2xl");
    public ColorRect VisualCollision;
    public override void _Ready()
    {
        base._Ready();
        Timer = new Timer();
        AddChild(Timer);
        Parent = GetParent<Level2>();

        windowSize = Size;

        VisualCollision = VisualCollisionScene.Instantiate<ColorRect>();
        VisualCollision.Size = Size;
        VisualCollision.Visible = false;
        GameManager.GameRoot.AddChild(VisualCollision);
    }

    public void ShowVisualCollision(Vector2I size, Vector2 position)
    {
        VisualCollision.Position = position;
        VisualCollision.Size = size;
        VisualCollision.Visible = true;
    }

    public void HideVisualCollision()
    {
        VisualCollision.Visible = false;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
        if (Attacking && !Shaking && IsCollided(Parent.CursorWindow))
		{
			Parent.CursorWindow.TakeDamage();
		}
    }
}