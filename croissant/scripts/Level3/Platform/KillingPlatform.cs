using Godot;

public partial class KillingPlatform : Platform
{
    [Export] public Area2D area2D;

    public override void _Ready()
    {
        base._Ready();
        area2D.BodyEntered += OnBodyEntered;
    }

    private void OnBodyEntered(Node body)
    {
        if (body is PlayerCharacter player)
        {
            CallDeferred(nameof(HandlePlayerDeath), player);
        }
    }

    private void HandlePlayerDeath(PlayerCharacter player)
    {
        var nextScene = Level3.Instance.level3Scenes[Level3.Instance.sceneid].Instantiate<SubLevel3>();
        Level3.Instance.sceneid = 0;
        
        // Use SetDeferred for physics-related changes
        player.SetDeferred("position", new Vector2(1846, 610));
        
        // Use CallDeferred for scene changes
        CallDeferred(nameof(UpdateScene), nextScene);
    }

    private void UpdateScene(SubLevel3 nextScene)
    {
        if (Level3.Instance.actualScene != null)
        {
            Level3.Instance.actualScene.QueueFree();
        }
        Level3.Instance.AddChild(nextScene);
        Level3.Instance.actualScene = nextScene;
        //GD.Print("You are dead!");
    }
}