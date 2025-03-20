using Godot;

[Tool]
public partial class Eye : Node2D
{
    private Vector2 previousPosition = Vector2.Zero;

	private Vector2 InitialPosition = Vector2.Zero;
    private Vector2 velocity = Vector2.Zero;
    private float distanceThreshold = 0.0f;

    [Export(PropertyHint.Range, "0.1, 2048.0")] private float eyeRadius = 936.0f;
    [Export(PropertyHint.Range, "0.1, 2048.0")] private float irisRadius = 572.0f;
    [Export(PropertyHint.Range, "0.01, 0.5")] private float irisEdgeMargin = 0.05f;

    [Export(PropertyHint.Range, "0.0, 10.0")] private float velocityDamping = 1.0f;
    [Export(PropertyHint.Range, "0.0, 10.0")] private float irisFriction = 0.2f;
    [Export] private Vector2 gravity = new Vector2(0, 1000);

    public override void _Ready()
    {
        UpdateThreshold();
		InitialPosition = GlobalPosition;
    }

    private void SetEyeRadius(float value)
    {
        eyeRadius = value;
        if (eyeRadius < irisRadius)
            irisRadius = eyeRadius;
        
        if (!Engine.IsEditorHint())
        {
            AwaitReady();
        }
        GetNode<Node2D>("white").Scale = Vector2.One * (eyeRadius / 936.0f);
        GetNode<Node2D>("gloss").Scale = GetNode<Node2D>("white").Scale;
        UpdateThreshold();
    }

    private void SetIrisRadius(float value)
    {
        irisRadius = Mathf.Min(value, eyeRadius);
        if (!Engine.IsEditorHint())
        {
            AwaitReady();
        }
        GetNode<Node2D>("black").Scale = Vector2.One * (irisRadius / 572.0f);
        UpdateThreshold();
    }

    private void SetIrisMargin(float value)
    {
        irisEdgeMargin = value;
        UpdateThreshold();
    }

    private void SetGravity(Vector2 value)
    {
        gravity = value;
    }

    private void UpdateThreshold()
    {
        previousPosition = GlobalPosition;
        distanceThreshold = -(irisRadius - eyeRadius) * (0.5f - irisEdgeMargin);
    }

    public override void _Process(double d)
    {
		float delta = (float)d;
        velocity -= (previousPosition - GlobalPosition) * irisFriction;
        GetNode<Node2D>("black").Position += (previousPosition - GlobalPosition);
        GetNode<Node2D>("black").Position += velocity * delta;

        if (GetNode<Node2D>("black").Position.Length() > distanceThreshold)
        {
            velocity -= ((GetNode<Node2D>("black").Position.Length() - distanceThreshold) *
                         GetNode<Node2D>("black").Position.Normalized()) / delta;
            GetNode<Node2D>("black").Position = distanceThreshold * GetNode<Node2D>("black").Position.Normalized();
        }

        velocity -= velocity * velocityDamping * delta;
        previousPosition = GlobalPosition;
    }

    private async void AwaitReady()
    {
        await ToSignal(this, "ready");
    }
}
