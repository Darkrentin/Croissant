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
        SubLevel3 MainScene = Level3.Instance.Level3Nodes[0];
        
        Level3.Instance.actualScene.HideSubLevel();
        Level3.Instance.sceneid = 0;
        MainScene.ShowSubLevel();
        Level3.Instance.actualScene = MainScene;
        // Use SetDeferred for physics-related changes
        player.SetDeferred("position", new Vector2(1846, 610));
        
        // Use CallDeferred for scene changes
    } //GD.Print("You are dead!");
}