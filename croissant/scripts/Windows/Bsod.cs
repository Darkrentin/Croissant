using Godot;

public partial class Bsod : Window
{
	[Export] public Button RestartButton;
	[Export] public AnimationPlayer AnimationPlayer;

	public override void _Ready()
	{
		RestartButton.Pressed += () => { AnimationPlayer.Play("Restart"); };
		AnimationPlayer.AnimationFinished += AnimationFinished;
	}

	public override void _Process(double delta)
	{

	}

	public void AnimationFinished(StringName animationName)
	{
		GameManager.State = GameManager.GameState.IntroHelper;
	}
}