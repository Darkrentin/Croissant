using Godot;
using System;

public partial class Level3 : FloatWindow
{
    [Export] public PackedScene[] level3Scenes;
    [Export] public PlayerCharacter player;
    [Export] public int MaxFiles = 4;
    public SubLevel3[] Level3Nodes;
    public int sceneid = 0;
    public static Level3 Instance;
    public SubLevel3 actualScene;
    public Action<InputEventMouseButton> MouseEvent;
    public int FilesCollected = 0;
    private Timer invincibleTimer;

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

        player.Position = GameManager.ScreenSize / 2;

        invincibleTimer = new Timer();
        invincibleTimer.WaitTime = 1.0f;
        invincibleTimer.OneShot = true;
        AddChild(invincibleTimer);
        invincibleTimer.Timeout += OnInvincibleTimerTimeout;
        invincibleTimer.Start();
    }

    public void ShowPlayer()
    {
        player.Position = GameManager.ScreenSize / 2;
        player.Visible = true;
        player.ProcessMode = ProcessModeEnum.Pausable;
    }

    private void OnInvincibleTimerTimeout()
    {
        player.isInvincible = false;
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
        if (!HasFocus()) GrabFocus();
    }


    public void Transition(int NextSceneId)
    {
        int nextSceneId = NextSceneId;
        if (NextSceneId == -1)
            nextSceneId = 0;
        player.isInvincible = true;
        invincibleTimer.Start();
        SubLevel3 nextScene = Level3.Instance.Level3Nodes[nextSceneId];

        if (Level3.Instance.actualScene != null)
        {
            Level3.Instance.actualScene.HideSubLevel();
        }
        nextScene.ShowSubLevel();
        Vector2 playerTargetPosition = GameManager.ScreenSize / 2;
        if (nextScene.HasNode($"{sceneid}") && NextSceneId!=-1)
            playerTargetPosition = nextScene.GetNode<Portal>($"{sceneid}").GlobalPosition + new Vector2(60, 60);
        Tween tween = GetTree().CreateTween();
        float distance = (player.GlobalPosition - playerTargetPosition).Length();
        float screenWidth = GameManager.ScreenSize.X;


        float duration = Math.Max(distance / (float)screenWidth, 0.1f);
        tween.SetTrans(Tween.TransitionType.Sine);
        tween.SetEase(Tween.EaseType.InOut);
        tween.TweenProperty(player, "global_position", playerTargetPosition, duration);
        tween.TweenCallback(Callable.From(() =>
        {
            player.isDead = false;
        }));
        sceneid = nextSceneId;
        actualScene = nextScene;
        GD.Print("Level complete!");

        if (player.Velocity.Y > 0)
        {
            player.Velocity = new Vector2(player.Velocity.X, 0);
        }
    }

    public void CollectFile()
    {
        FilesCollected++;
        if (FilesCollected >= MaxFiles)
        {
            GameManager.State = GameManager.GameState.Dialogue3;
            Transition(0);
            GetTree().CreateTimer(0.5f).Timeout += () =>
            {
                player.LevelEnd = true;
            };
        }
    }

}
