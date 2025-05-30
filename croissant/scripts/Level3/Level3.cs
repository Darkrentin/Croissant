using Godot;
using System;

public partial class Level3 : FloatWindow
{
    [Export] public PackedScene[] level3Scenes;
    [Export] public PlayerCharacter player;
    [Export] public int MaxFiles = 4;

    [Export] public AudioStreamPlayer ConfigFileGatheredSound;
    [Export] public AudioStreamPlayer PortalEnterSound;
    [Export] public AudioStreamPlayer PortalExitSound;

    public SubLevel3[] Level3Nodes;
    public int sceneid = 0;
    public static Level3 Instance;
    public SubLevel3 actualScene;
    public Action<InputEventMouseButton> MouseEvent; public int FilesCollected = 0;
    private Timer invincibleTimer;
    private bool[] loadedScenes;

    public override void _Ready()
    {
        base._Ready();
        Title = "Level 3";
        GameManager.MainWindow.ContentScaleMode = ContentScaleModeEnum.CanvasItems;
        GameManager.MainWindow.ContentScaleAspect = ContentScaleAspectEnum.Ignore;
        GrabFocus();
        Instance = this;

        Level3Nodes = new SubLevel3[level3Scenes.Length];
        loadedScenes = new bool[level3Scenes.Length];
        LoadSceneImmediate(0);
        actualScene = Level3Nodes[sceneid];
        actualScene.ShowSubLevel();

        LoadAdjacentScenes(0);

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
        GrabFocus();
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
        base._Process(delta);
    }

    private void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= level3Scenes.Length || loadedScenes[sceneIndex])
            return;

        CallDeferred(nameof(LoadSceneDeferred), sceneIndex);
    }

    private void LoadSceneDeferred(int sceneIndex)
    {
        if (loadedScenes[sceneIndex])
            return;

        Level3Nodes[sceneIndex] = level3Scenes[sceneIndex].Instantiate<SubLevel3>();
        AddChild(Level3Nodes[sceneIndex]);
        Level3Nodes[sceneIndex].HideSubLevel();
        loadedScenes[sceneIndex] = true;
    }

    // Load a single scene immediately
    private void LoadSceneImmediate(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= level3Scenes.Length || loadedScenes[sceneIndex])
            return;

        Level3Nodes[sceneIndex] = level3Scenes[sceneIndex].Instantiate<SubLevel3>();
        AddChild(Level3Nodes[sceneIndex]);
        Level3Nodes[sceneIndex].HideSubLevel();
        loadedScenes[sceneIndex] = true;
    }

    private void LoadAdjacentScenes(int currentSceneId)
    {
        if (currentSceneId == 0)
        {
            // Level 0: load portals 1, 6, 11, 16
            LoadScene(1);
            LoadScene(6);
            LoadScene(11);
            LoadScene(16);
        }
        else if (currentSceneId >= 1 && currentSceneId <= 5)
        {
            if (currentSceneId < 5)
                LoadScene(currentSceneId + 1);
        }
        else if (currentSceneId >= 6 && currentSceneId <= 10)
        {
            if (currentSceneId < 10)
                LoadScene(currentSceneId + 1);
        }
        else if (currentSceneId >= 11 && currentSceneId <= 15)
        {
            if (currentSceneId < 15)
                LoadScene(currentSceneId + 1);
        }
        else if (currentSceneId >= 16 && currentSceneId <= 20)
        {
            if (currentSceneId < 20)
                LoadScene(currentSceneId + 1);
        }
    }

    public void Transition(int NextSceneId)
    {
        int nextSceneId = NextSceneId;
        if (NextSceneId == -1)
            nextSceneId = 0;
        PortalEnterSound.Play();
        LoadScene(nextSceneId);

        player.isInvincible = true;
        invincibleTimer.Start();

        CallDeferred(nameof(TransitionDeferred), nextSceneId, NextSceneId);
    }

    private void TransitionDeferred(int nextSceneId, int originalNextSceneId)
    {
        SubLevel3 nextScene = Level3.Instance.Level3Nodes[nextSceneId];

        if (Level3.Instance.actualScene != null)
        {
            Level3.Instance.actualScene.HideSubLevel();
        }
        nextScene.ShowSubLevel();

        Vector2 playerTargetPosition = GameManager.ScreenSize / 2;
        if (nextScene.HasNode($"{sceneid}") && originalNextSceneId != -1)
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
            PortalExitSound.Play();
            GameManager.StartRefocusAllWindows();

        }));
        sceneid = nextSceneId;
        actualScene = nextScene;

        LoadAdjacentScenes(nextSceneId);

    }

    public void CollectFile()
    {
        FilesCollected++;
        ConfigFileGatheredSound.Play();
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
