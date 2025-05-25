using Godot;
using System;

public partial class SpatialStreamPlayer3D : AudioStreamPlayer3D
{
    [Export] public RayCast3D RayCast;
    [Export] public AudioStreamPlayer3D[] Sounds;
    [Export] public float VolumeReductionThroughWalls = -20.0f; // Volume reduction in dB
    [Export] public float UpdateFrequency = 0.1f; // Update frequency in seconds
    
    private float timer = 0.0f;
    private float[] originalVolumes;
    private bool[] soundsHaveObstacles; // Track obstacle state for each sound
    
    public override void _Ready()
    {
        // Save original volumes and initialize obstacle tracking
        if (Sounds != null)
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
    }

    // Use _PhysicsProcess instead of _Process for raycast operations
    public override void _PhysicsProcess(double delta)
    {
        timer += (float)delta;
        
        // Update volume based on defined frequency
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
            
        // Configure raycast towards the player
        Vector3 directionToPlayer = FinalLevel.Instance.Player3D.GlobalPosition - GlobalPosition;
        RayCast.TargetPosition = RayCast.ToLocal(GlobalPosition + directionToPlayer);

        // Check if there's a wall between player and audio source
        // Only muffle if raycast hits something that is NOT the player (i.e., a wall)
        bool hasObstacle = RayCast.IsColliding() && !(RayCast.GetCollider() is Player3D);
        
        // Debug to see what we're hitting
        if (RayCast.IsColliding())
        {
            var collider = RayCast.GetCollider();
            GD.Print($"Raycast hit: {collider?.GetType().Name}");
        }
        
        // Adjust volume of all sounds
        for (int i = 0; i < Sounds.Length; i++)
        {
            if (Sounds[i] != null && i < originalVolumes.Length)
            {
                // Check if obstacle state changed for this sound
                if (soundsHaveObstacles[i] != hasObstacle)
                {
                    soundsHaveObstacles[i] = hasObstacle;
                    
                    // Apply volume change with fade if sound is playing
                    if (Sounds[i].Playing)
                    {
                        ApplyVolumeWithFade(Sounds[i], hasObstacle, originalVolumes[i]);
                    }
                    else
                    {
                        // Apply immediately if not playing
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
        
        // Fade duration based on whether we're muffling or clearing
        float fadeDuration = hasObstacle ? 0.5f : 0.3f;
        
        tween.TweenProperty(audioPlayer, "volume_db", targetVolume, fadeDuration);
    }
}
