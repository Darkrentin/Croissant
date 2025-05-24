using System.Runtime.CompilerServices;
using Godot;

public partial class Bsod : Window
{
	[Export] public AudioStreamPlayer BsodSound;
	[Export] public AudioStreamPlayer WindowsStartupSound;
	[Export] public Button RestartButton;
	[Export] public AnimationPlayer AnimationPlayer;
	public static Bsod Instance;

	public override void _Ready()
	{
		Instance = this;
		BsodSound.Play();
		RestartButton.Pressed += () => { AnimationPlayer.Play("Restart"); WindowsStartupSound.Play(); };
		AnimationPlayer.AnimationFinished += AnimationFinished;
	}

	public void AnimationFinished(StringName animationName)
	{
		QueueFree();
		GameManager.State = GameManager.GameState.IntroHelper;
	}
}
