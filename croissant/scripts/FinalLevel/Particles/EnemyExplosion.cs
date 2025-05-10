using Godot;

public partial class EnemyExplosion : GpuParticles3D
{
	[Export] private OmniLight3D ExplosionLight;

	public override void _Ready()
	{
		Emitting = true;
		Tween lightTween = CreateTween();
		lightTween.TweenProperty(ExplosionLight, "light_energy", 0f, 1.2f);
		lightTween.Play();
	}

	public void _on_timer_timeout()
	{
		QueueFree();
	}
}