using Godot;

public partial class Platform : CharacterBody2D
{
    [Export] public WindowPlatform window;
    [Export] public CollisionShape2D collisionShape;
    [Export] public Vector2 BaseSpeeds = new Vector2(20f, 20f);
    [Export] public bool Freeze = true;
    [Export] public ColorRect Texture;

    public Vector2 MouseOffset;
    public RectangleShape2D Shape;
    public ShaderMaterial Shader;
    public Vector2I CachedTitleBarSize;
    public bool WindowValid;

    public override void _Ready()
    {
        Shape = collisionShape.Shape as RectangleShape2D;
        WindowValid = IsInstanceValid(window);

        if (WindowValid)
        {
            CachedTitleBarSize = window.TitleBarSize;
            window.Size = (Vector2I)((Shape.Size - CachedTitleBarSize) * Lib.GetScreenRatio());
            window.Position = (Vector2I)GlobalPosition + CachedTitleBarSize;

            if (!GameManager.Windows.Contains(window))
            {
                GameManager.Windows.Add(window);
            }

            window.DeleteWindow = () =>
            {
                GameManager.Windows.Remove(window);
            };
        }

        window.Title = "";
        base._Ready();
    }

    public override void _PhysicsProcess(double delta)
    {
        if (WindowValid)
        {
            window.Position = (Vector2I)((GlobalPosition + CachedTitleBarSize) * Lib.GetScreenRatio());
        }
        base._PhysicsProcess(delta);
    }
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _ExitTree()
    {
        if (WindowValid)
        {
            GameManager.Windows.Remove(window);
            window.QueueFree();
        }
        base._ExitTree();
    }
}
