using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

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
    private Timer invincibleTimer;    private bool[] loadedScenes;
    private Dictionary<int, ResourceLoader.ThreadLoadStatus> loadingStatus;
    private HashSet<int> requestedLoads;
    public bool End = false;

    public override void _Ready()
    {
        base._Ready();
        Title = "Level 3";
        GameManager.MainWindow.ContentScaleMode = ContentScaleModeEnum.CanvasItems;
        GameManager.MainWindow.ContentScaleAspect = ContentScaleAspectEnum.Ignore;
        GrabFocus();
        Instance = this;        Level3Nodes = new SubLevel3[level3Scenes.Length];
        loadedScenes = new bool[level3Scenes.Length];
        loadingStatus = new Dictionary<int, ResourceLoader.ThreadLoadStatus>();
        requestedLoads = new HashSet<int>();
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
        }    }
    
    public override void _ExitTree()
    {
        base._ExitTree();
        MouseEvent = null;
    }

    private void LoadScene(int sceneIndex)
    {
        if (sceneIndex < 0 || sceneIndex >= level3Scenes.Length || 
            loadedScenes[sceneIndex] || requestedLoads.Contains(sceneIndex))
            return;

        requestedLoads.Add(sceneIndex);
        
        // Start threaded loading
        string scenePath = level3Scenes[sceneIndex].ResourcePath;
        ResourceLoader.LoadThreadedRequest(scenePath);
        loadingStatus[sceneIndex] = ResourceLoader.ThreadLoadStatus.InProgress;
        
        Lib.Print($"Started threaded loading for scene {sceneIndex}: {scenePath}");
    }

    public override void _Process(double delta)
    {
        if (!HasFocus()) GrabFocus();
        
        // Check threaded loading progress
        CheckThreadedLoading();
        
        base._Process(delta);
    }

    private void CheckThreadedLoading()
    {
        var completedLoads = new List<int>();
        
        foreach (var kvp in loadingStatus.ToArray())
        {
            int sceneIndex = kvp.Key;
            var status = kvp.Value;
            
            if (status == ResourceLoader.ThreadLoadStatus.InProgress)
            {
                string scenePath = level3Scenes[sceneIndex].ResourcePath;
                var currentStatus = ResourceLoader.LoadThreadedGetStatus(scenePath);
                loadingStatus[sceneIndex] = currentStatus;
                
                if (currentStatus == ResourceLoader.ThreadLoadStatus.Loaded)
                {
                    completedLoads.Add(sceneIndex);
                }
                else if (currentStatus == ResourceLoader.ThreadLoadStatus.Failed)
                {
                    Lib.Print($"Failed to load scene {sceneIndex}: {scenePath}");
                    requestedLoads.Remove(sceneIndex);
                    loadingStatus.Remove(sceneIndex);
                }
            }
        }
        
        // Process completed loads
        foreach (int sceneIndex in completedLoads)
        {
            CompleteSceneLoad(sceneIndex);
        }
    }

    private void CompleteSceneLoad(int sceneIndex)
    {
        try
        {
            string scenePath = level3Scenes[sceneIndex].ResourcePath;
            var loadedResource = ResourceLoader.LoadThreadedGet(scenePath);
            
            if (loadedResource is PackedScene packedScene)
            {
                Level3Nodes[sceneIndex] = packedScene.Instantiate<SubLevel3>();
                AddChild(Level3Nodes[sceneIndex]);
                Level3Nodes[sceneIndex].HideSubLevel();
                loadedScenes[sceneIndex] = true;
                
                Lib.Print($"Successfully loaded scene {sceneIndex} via threading");
            }
            else
            {
                Lib.Print($"Scene {sceneIndex} loaded but is not a PackedScene");
            }
        }
        catch (Exception ex)
        {
            Lib.Print($"Error completing load for scene {sceneIndex}: {ex.Message}");
        }
        finally
        {
            requestedLoads.Remove(sceneIndex);
            loadingStatus.Remove(sceneIndex);
        }
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

    public void Transition(int NextSceneId, Portal portal)
    {
        PortalEnterSound.Play();
        LoadScene(NextSceneId);

        player.isInvincible = true;
        invincibleTimer.Start();

        portal.AnimationPlayer.Play("Close");
        player.Animator2.Play("Hide");

        GetTree().CreateTimer(0.31f).Timeout += () =>
        {
            TransitionDeferred(NextSceneId, portal);
        };
    }

    private void TransitionDeferred(int nextSceneId, Portal portal)
    {
        SubLevel3 nextScene = Level3.Instance.Level3Nodes[nextSceneId];

        Portal NextPortal = null;
        if (!portal.PortalToSpawn)
        {
            NextPortal = nextScene.GetNode<Portal>($"{sceneid}");
            NextPortal.AnimationPlayer.Play("CloseInit");
            portal.AnimationPlayer.Play("OpenInit");
        }

        if (Level3.Instance.actualScene != null)
        {
            Level3.Instance.actualScene.HideSubLevel();
        }
        nextScene.ShowSubLevel();

        Vector2 playerTargetPosition = GameManager.ScreenSize / 2;

        if (!portal.PortalToSpawn)
        {
            playerTargetPosition = NextPortal.GlobalPosition + new Vector2(60, 60);
            player.GlobalPosition = playerTargetPosition;
            player.Animator2.PlayBackwards("Hide");
            NextPortal.AnimationPlayer.PlayBackwards("Close");
            

            GetTree().CreateTimer(0.3f).Timeout += () =>
            {
                player.isDead = false;
                player.Visible = true;
                PortalExitSound.Play();
                //CallDeferred(nameof(GrabFocus));
            };
        }
        else
        {
            // Portal PortalToSpawn : supprimer le portail et le chemin du niveau 0
            CleanupPathAndPortal(sceneid);

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
                player.Visible = true;
                player.Animator2.PlayBackwards("Hide");
                //CallDeferred(nameof(GrabFocus));
            }));
        }

        if (End)
        {
            EndLevel();
        }

        sceneid = nextSceneId;
        actualScene = nextScene;

        LoadAdjacentScenes(nextSceneId);
    }
    
    public void TransitionStuck()
    {
        player.isDead = true;
        player.Visible = false;
        SubLevel3 nextScene = Level3.Instance.Level3Nodes[0];
        if (Level3.Instance.actualScene != null)
        {
            Level3.Instance.actualScene.HideSubLevel();
        }
        nextScene.ShowSubLevel();

        GetTree().CreateTimer(0.3f).Timeout += () =>
        {
            player.isDead = false;
            player.Visible = true;
            player.GlobalPosition = GameManager.ScreenSize / 2;
            //CallDeferred(nameof(GrabFocus));
        };  
        

        if (End)
        {
            EndLevel();
        }

        sceneid = 0;
        actualScene = nextScene;

        LoadAdjacentScenes(0);
    }

    private void CleanupPathAndPortal(int currentSceneId)
    {
        // Déterminer quel portail du niveau 0 doit être supprimé
        string portalToRemove = "";
        int[] pathToClean = null;

        if (currentSceneId >= 1 && currentSceneId <= 5)
        {
            portalToRemove = "1";
            pathToClean = new int[] { 1, 2, 3, 4, 5 };
            Lib.Print("Cleaning path 1-5");
        }
        else if (currentSceneId >= 6 && currentSceneId <= 10)
        {
            portalToRemove = "6";
            pathToClean = new int[] { 6, 7, 8, 9, 10 };
            Lib.Print("Cleaning path 6-10");
        }
        else if (currentSceneId >= 11 && currentSceneId <= 15)
        {
            portalToRemove = "11";
            pathToClean = new int[] { 11, 12, 13, 14, 15 };
            Lib.Print("Cleaning path 11-15");
        }
        else if (currentSceneId >= 16 && currentSceneId <= 20)
        {
            portalToRemove = "16";
            pathToClean = new int[] { 16, 17, 18, 19, 20 };
            Lib.Print("Cleaning path 16-20");
        }

        if (portalToRemove != "" && pathToClean != null)
        {
            // Supprimer le portail du niveau 0
            RemovePortalFromLevel0(portalToRemove);

            // Libérer tous les niveaux du chemin
            FreePathLevels(pathToClean);
        }
    }

    private void RemovePortalFromLevel0(string portalName)
    {
        if (loadedScenes[0] && Level3Nodes[0] != null)
        {
            try
            {
                Portal portalToRemove = Level3Nodes[0].GetNode<Portal>(portalName);
                if (portalToRemove != null)
                {
                    Lib.Print($"Removing portal {portalName} from level 0");
                    portalToRemove.GetParent().RemoveChild(portalToRemove);
                    portalToRemove.QueueFree();
                }
            }
            catch (System.Exception ex)
            {
                Lib.Print($"Portal {portalName} not found in level 0: {ex.Message}");
            }
        }
    }

    private void FreePathLevels(int[] pathLevels)
    {
        foreach (int levelIndex in pathLevels)
        {
            if (levelIndex >= 0 && levelIndex < Level3Nodes.Length && 
                loadedScenes[levelIndex] && Level3Nodes[levelIndex] != null)
            {
                Lib.Print($"Freeing level {levelIndex}");
                
                // Marquer comme non chargé
                loadedScenes[levelIndex] = false;
                
                // Retirer de la scène et libérer
                Level3Nodes[levelIndex].GetParent().RemoveChild(Level3Nodes[levelIndex]);
                Level3Nodes[levelIndex].QueueFree();
                Level3Nodes[levelIndex] = null;
            }
        }
    }

    // Optionnel : méthode pour vérifier l'état des niveaux chargés
    public void DebugLoadedScenes()
    {
        Lib.Print("=== Loaded Scenes Status ===");
        for (int i = 0; i < loadedScenes.Length; i++)
        {
            if (loadedScenes[i])
            {
                Lib.Print($"Level {i}: LOADED");
            }
            else
            {
                Lib.Print($"Level {i}: FREE");
            }
        }
    }

    public void CollectFile()
    {
        FilesCollected++;
        ConfigFileGatheredSound.Play();
        if (FilesCollected >= MaxFiles)
        {
            End = true;

        }
    }

    public void EndLevel()
    {
        GameManager.State = GameManager.GameState.Dialogue3;
        GetTree().CreateTimer(0.5f).Timeout += () =>
        {
            player.LevelEnd = true;
        };
    }
}
