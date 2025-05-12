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

	
}
