using Godot;

public partial class Eye : Node2D
{
	[Export] public Vector2 MaxEyeDistance = new Vector2(100f, 100f);
	[Export] public Node2D black;
	public Virus virus;

	public override void _Ready()
	{
		virus = GameManager.virus;
	}

	public override void _Process(double d)
	{
		Vector2I cursorPosition = Lib.GetCursorPosition();
		Vector2I centerPosition = virus.Position + virus.Size / 2;
		Vector2I relativePosition = centerPosition - cursorPosition;

		float normalizedX = relativePosition.X / (GameManager.ScreenSize.X / 2.0f);
		float normalizedY = relativePosition.Y / (GameManager.ScreenSize.Y / 2.0f);

		normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f);
		normalizedY = Mathf.Clamp(normalizedY, -1.0f, 1.0f);

		float positionX = -normalizedX * MaxEyeDistance.X;
		float positionY = -normalizedY * MaxEyeDistance.Y;

		black.Position = new Vector2(positionX, positionY);
	}
}