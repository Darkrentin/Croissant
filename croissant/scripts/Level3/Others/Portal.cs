using System.Runtime.Intrinsics.Wasm;
using Godot;

public partial class Portal : Area2D
{
	[Export] public int NextSceneId = 1;
	public Timer timer;
	public bool isChangingScene = false;
	public override void _Ready()
	{
		// Connect the body entered signal to the OnBoddyEntered method
		BodyEntered += OnBoddyEntered;
		timer = new Timer();
		timer.WaitTime = 0.5f;
		timer.OneShot = true;
		timer.Timeout += () =>
		{
			isChangingScene = false;
			timer.Stop();
		};
		VisibilityChanged += OnVisibilityChange;
		AddChild(timer);

	}

	public void OnBoddyEntered(Node body)
	{
		if(isChangingScene)
			return;
		
		if (body is PlayerCharacter player)
		{
			GD.Print("OnBoddyEntered");
			OnLevelComplete(player);
			
			
		}
	}

	public void OnVisibilityChange()
	{
		if(Visible)
		{
			timer.Start();
			isChangingScene = true;
		}
	}

	private void OnLevelComplete(PlayerCharacter player)
	{
		SubLevel3 nextScene = Level3.Instance.Level3Nodes[NextSceneId];	
		
		if (Level3.Instance.actualScene != null)
		{
			Level3.Instance.actualScene.HideSubLevel();
		}
		nextScene.ShowSubLevel();
		player.GlobalPosition = nextScene.GetNode<Portal>($"{Level3.Instance.sceneid}").GlobalPosition + new Vector2(60,60);
		Level3.Instance.sceneid = NextSceneId;
		Level3.Instance.actualScene = nextScene;
		GD.Print("Level complete!");
	}

}
