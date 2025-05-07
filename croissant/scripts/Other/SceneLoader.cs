using Godot;

public partial class SceneLoader : Node
{
	public static SceneLoader Instance; 
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
	[Export] public PackedScene CursorWindowScene;
	[Export] public PackedScene LaserWindowScene;
	[Export] public PackedScene ExtendWindowScene;
	[Export] public PackedScene CompressWindowScene;
	[Export] public PackedScene FlappyWindowScene;
	[Export] public PackedScene FollowWindowScene;
	[Export] public PackedScene SpikeWindowScene;

	public override void _Ready()
	{
		SceneLoader.Instance = this;
	}

	public override void _Process(double delta)
	{

	}
}
