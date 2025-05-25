using Godot;

public partial class SpatialStreamPlayer3D : AudioStreamPlayer3D
{
    [Export] public RayCast3D RayCast;
    [Export] public AudioStreamPlayer3D[] Sounds;
    [Export] public float VolumeReductionThroughWalls = -20.0f;
    [Export] public float UpdateFrequency = 0.1f;
    private float timer = 0.0f;
    private float[] originalVolumes;
    private bool[] soundsHaveObstacles;

    public override void _Ready()
    {
        originalVolumes = new float[Sounds.Length];
        soundsHaveObstacles = new bool[Sounds.Length];

        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i] != null)
            {
                originalVolumes[i] = Sounds[i].VolumeDb;
                soundsHaveObstacles[i] = false;
            }
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        timer += (float)delta;
        if (timer >= UpdateFrequency)
        {
            timer = 0.0f;
            UpdateVolumeBasedOnObstacles();
        }
    }

    private void UpdateVolumeBasedOnObstacles()
    {
        if (FinalLevel.Instance?.Player3D == null || RayCast == null || Sounds == null)
            return;

        Vector3 directionToPlayer = FinalLevel.Instance.Player3D.GlobalPosition - GlobalPosition;
        RayCast.TargetPosition = RayCast.ToLocal(GlobalPosition + directionToPlayer);

        bool hasObstacle = RayCast.IsColliding() && !(RayCast.GetCollider() is Player3D);

        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i] != null && i < originalVolumes.Length)
            {
                if (soundsHaveObstacles[i] != hasObstacle)
                {
                    soundsHaveObstacles[i] = hasObstacle;
                    if (Sounds[i].Playing)
                    {
                        ApplyVolumeWithFade(Sounds[i], hasObstacle, originalVolumes[i]);
                    }
                    else
                    {
                        Sounds[i].VolumeDb = hasObstacle ?
                            originalVolumes[i] + VolumeReductionThroughWalls :
                            originalVolumes[i];
                    }
                }
            }
        }
    }

    private void ApplyVolumeWithFade(AudioStreamPlayer3D audioPlayer, bool hasObstacle, float originalVolume)
    {
        var tween = CreateTween();
        float targetVolume = hasObstacle ? originalVolume + VolumeReductionThroughWalls : originalVolume;
        float fadeDuration = hasObstacle ? 0.5f : 0.3f;

        tween.TweenProperty(audioPlayer, "volume_db", targetVolume, fadeDuration);
    }
}
