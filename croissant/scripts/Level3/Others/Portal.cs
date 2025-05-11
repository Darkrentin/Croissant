using System.Runtime.Intrinsics.Wasm;
using Godot;

public partial class Portal : Area2D
{
	[Export] public int NextSceneId = 1;
	[Export] public Node2D SpawnPosition;
	
	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		CheckForPlayerCollision();
	}

	private void CheckForPlayerCollision()
	{
		var overlappingBodies = GetOverlappingBodies();
		
		foreach (var body in overlappingBodies)
		{
			if (body is PlayerCharacter player)
			{
				CallDeferred(nameof(OnLevelComplete));
				player.SetDeferred("position",SpawnPosition.GlobalPosition);
				break;
			}
		}
	}

	private void OnLevelComplete()
	{
		Node nextScene = null;
		if(Level3.Instance.Level3Nodes[NextSceneId] != null)
		{
			nextScene = Level3.Instance.Level3Nodes[NextSceneId];
		}
		else
		{
			nextScene = Level3.Instance.level3Scenes[Level3.Instance.sceneid].Instantiate<Node>();
		}
		CallDeferred(nameof(CompleteLevel), nextScene);
	}

	private void CompleteLevel(Node nextScene)
	{
		Level3.Instance.sceneid = NextSceneId;
		CallDeferred(nameof(UpdateScene), nextScene);
	}

	private void UpdateScene(Node nextScene)
	{
		if (Level3.Instance.actualScene != null)
		{
			Level3.Instance.RemoveChild(Level3.Instance.actualScene);
		}
		Level3.Instance.AddChild(nextScene);
		Level3.Instance.actualScene = nextScene;
		GD.Print("Level complete!");
	}
}
