using Godot;

public partial class BulletHit : GpuParticles3D
{
	[Export] private OmniLight3D BulletHitLight;

	public override void _Ready()
	{
		Emitting = true;
		Tween lightTween = CreateTween();
		lightTween.TweenProperty(BulletHitLight, "light_energy", 0f, 0.3f);
		lightTween.Play();
	}

	void _on_timer_timeout()
	{
		QueueFree();
	}
}