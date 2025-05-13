using Godot;
using System;

public partial class Level3 : FloatWindow
{
    [Export] public PackedScene[] level3Scenes;
    public SubLevel3[] Level3Nodes;
    public int sceneid = 0;
    public static Level3 Instance;
    public SubLevel3 actualScene;
    public Action<InputEventMouseButton> MouseEvent;
    [Export] public PlayerCharacter player;
    [Export] public int MaxFiles = 5;
    public int FilesCollected = 0;

    public override void _Ready()
    {
        GrabFocus();
        Instance = this;
        Level3Nodes = new SubLevel3[level3Scenes.Length];
        for (int i = 0; i < level3Scenes.Length; i++)
        {
            Level3Nodes[i] = level3Scenes[i].Instantiate<SubLevel3>();
            AddChild(Level3Nodes[i]);
            Level3Nodes[i].HideSubLevel();
        }
        actualScene = Level3Nodes[sceneid];
        actualScene.ShowSubLevel();
    }
    

    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseButtonEvent && MouseEvent != null)
        {
            try
            {
                MouseEvent?.Invoke(mouseButtonEvent);
            }
            catch (ObjectDisposedException)
            {
                // Clean up the disposed window reference
                MouseEvent = null;
            }
        }
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        MouseEvent = null;
    }

    public override void _Process(double delta)
    {
		if(!HasFocus()) GrabFocus();
    }


    public void Transition(Portal Portal)
    {
        SubLevel3 nextScene = Level3.Instance.Level3Nodes[Portal.NextSceneId];	
        player.ProcessMode = ProcessModeEnum.Disabled;
		
		if (Level3.Instance.actualScene != null)
		{
			Level3.Instance.actualScene.HideSubLevel();
		}
		nextScene.ShowSubLevel();
        Vector2 playerTargetPosition = nextScene.GetNode<Portal>($"{sceneid}").GlobalPosition + new Vector2(60, 60);
        Tween tween = GetTree().CreateTween();
        float distance = (player.GlobalPosition - playerTargetPosition).Length();
        float screenWidth = GameManager.ScreenSize.X;
        float duration = distance / (float)screenWidth; // Time to cross full screen = 1 second
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(player, "global_position", playerTargetPosition, duration);
        tween.TweenCallback(Callable.From(() => {
            player.ProcessMode = ProcessModeEnum.Pausable;
        }));
		sceneid = Portal.NextSceneId;
		actualScene = nextScene;
		GD.Print("Level complete!");
    }

    public void CollectFile()
    {
        FilesCollected++;
        if (FilesCollected >= MaxFiles)
        {
            GameManager.State = GameManager.GameState.Void;
            GetParent().RemoveChild(this);
            QueueFree();
        }
    }
	
}
