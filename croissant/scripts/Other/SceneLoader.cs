using Godot;

public partial class SceneLoader : Node
{
	[Export] public PackedScene DifficultyScene;
	[Export] public PackedScene IntroGameScene;
	[Export] public PackedScene Level1Scene;
	[Export] public PackedScene Level2Scene;
	[Export] public PackedScene VirusScene;
	[Export] public PackedScene HelperScene;
	[Export] public PackedScene StaticWindowScene;
	[Export] public PackedScene TimerWindowScene;
	[Export] public PackedScene MoveWindowScene;
	[Export] public PackedScene DodgeWindowScene;
	[Export] public PackedScene TankWindowScene;
	[Export] public PackedScene BombWindowScene;
	[Export] public PackedScene VirusSplashScene;
	[Export] public PackedScene BsodScene;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

	}
}
