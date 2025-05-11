using Godot;
using System;

public partial class Level3 : FloatWindow
{
    [Export] public PackedScene[] level3Scenes;
    public Node[] Level3Nodes;
    public int sceneid = 0;
    public static Level3 Instance;
    public Node actualScene;
    public Action<InputEventMouseButton> MouseEvent;
    [Export] public PlayerCharacter player;

    public override void _Ready()
    {
        GrabFocus();
        Instance = this;
        actualScene = level3Scenes[sceneid].Instantiate<Node>();
        Level3Nodes = new Node[level3Scenes.Length];
        for (int i = 0; i < level3Scenes.Length; i++)
        {
            Level3Nodes[i] = null;
        }
        AddChild(actualScene);
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
