using System.Collections.Generic;
using System.IO;
using Godot;

public partial class Maze : Node3D
{
    [Export] public PackedScene WallScene;
    [Export] public PackedScene FloorScene;
    [Export] public PackedScene LampScene;
    [Export] public PackedScene ObjectiveScene;
    [Export] public PackedScene EasterScene;
    [Export] public PackedScene FlashLightScene;
    [Export] public PackedScene HelperBodyScene;
    [Export] public AudioStreamPlayer WallSound;
    [Export] public int MazeSize = 31;

    public Node3D HelperBody;
    public Easter EasterEggs;
    public Node3D FlashLight;


    public int WallSize = 2;
    public const int LampSpacing = 5;

    public const int Lamp = -2;
    public const int CantSpawn = -1;
    public const int Floor = 0;
    public const int Wall = 1;
    public const int ObjectiveLabel = 2;
    public const int EasterEgg = 3;

    public Color[] ObjectiveColors = new Color[]
    {
        Colors.Magenta,
        Colors.Green,
        Colors.Blue,
    };

    public int[,] MazeData;
    public int[,] MazeDist;
    public StaticBody3D[,] MazeTiles;

    public List<Node3D> Lamps = new List<Node3D>();
    public List<Enemy3D> Enemies = new List<Enemy3D>();
    public List<StaticBody3D> Paths = new List<StaticBody3D>();
    public List<Objective> Objectives = new List<Objective>();
    public bool WallRemove = false;
    [Export(PropertyHint.Range, "0.0,1.0,0.01")] public float LoopCreationProbability = 0.1f;

    public override void _Ready()
    {
        if (MazeSize % 2 == 0)
            MazeSize++;
    }

    public void CreateMaze()
    {
        initMaze();
        DisplayMaze();
        MakeMaze();
    }



    public void initMaze()
    {
        MazeData = new int[MazeSize, MazeSize];
        MazeDist = new int[MazeSize, MazeSize];
        MazeTiles = new StaticBody3D[MazeSize, MazeSize];

        for (int i = 0; i < MazeSize; i++)
            for (int j = 0; j < MazeSize; j++)
            {
                MazeData[i, j] = Wall;
                MazeDist[i, j] = -1;
                MazeTiles[i, j] = null;
            }

        if (MazeSize > 2)
            GenerateMazeDFS(1, 1);

        PlaceRoom(MazeSize / 2, MazeSize / 2, 5, 5, true);
        const int SafeZone = 11;
        ReplaceLabel(MazeSize / 2, MazeSize / 2, SafeZone, SafeZone, Floor, CantSpawn, true);


        (int x, int y)[] corners = new (int, int)[]
        {
            (2, 2),
            (MazeSize - 3, 2),
            (2, MazeSize - 3),
            (MazeSize - 3, MazeSize - 3)
        };

        int cornerToRemoveIndex = Lib.rand.Next(0, corners.Length);
        Vector2I cornerToRemove = new Vector2I(corners[cornerToRemoveIndex].x, corners[cornerToRemoveIndex].y);
        corners[cornerToRemoveIndex] = (0, 0);

        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i] != (0, 0))
            {
                PlaceRoom(corners[i].x, corners[i].y, 3, 3, true);
                MazeData[corners[i].x, corners[i].y] = ObjectiveLabel;
            }
        }
        PlaceRoom(cornerToRemove.X, cornerToRemove.Y, 3, 3, true);
        MazeData[cornerToRemove.X, cornerToRemove.Y] = EasterEgg;

        MazeData[MazeSize / 2 - 1, MazeSize / 2 - 1] = ObjectiveLabel;
        MazeData[MazeSize / 2 - 1, MazeSize / 2] = ObjectiveLabel;
        MazeData[MazeSize / 2, MazeSize / 2 - 1] = ObjectiveLabel;

        CalculateDistancesFromCenter();
    }

    private void GenerateMazeDFS(int r, int c)
    {
        MazeData[r, c] = Floor;

        int[] directions = { 0, 1, 2, 3 };
        Lib.rand.Shuffle(directions);

        foreach (int dir in directions)
        {
            int nextR = r;
            int nextC = c;
            int wallR = r;
            int wallC = c;

            switch (dir)
            {
                case 0: nextR -= 2; wallR -= 1; break;
                case 1: nextC += 2; wallC += 1; break;
                case 2: nextR += 2; wallR += 1; break;
                case 3: nextC -= 2; wallC -= 1; break;
            }

            if (wallR > 0 && wallR < MazeSize - 1 && wallC > 0 && wallC < MazeSize - 1)
            {
                if (nextR > 0 && nextR < MazeSize - 1 && nextC > 0 && nextC < MazeSize - 1 &&
                    MazeData[nextR, nextC] == Wall)
                {
                    MazeData[wallR, wallC] = Floor;
                    GenerateMazeDFS(nextR, nextC);
                }
                else if (MazeData[wallR, wallC] == Wall &&
                         nextR > 0 && nextR < MazeSize - 1 && nextC > 0 && nextC < MazeSize - 1 &&
                         (MazeData[nextR, nextC] == Floor))
                {
                    if (Lib.rand.NextDouble() < LoopCreationProbability)
                        MazeData[wallR, wallC] = Floor;
                }
            }
        }
    }

    private void CalculateDistancesFromCenter()
    {
        Queue<(int, int)> queue = new Queue<(int, int)>();
        int centerX = MazeSize / 2;
        int centerY = MazeSize / 2;

        MazeDist[centerX, centerY] = 0;
        queue.Enqueue((centerX, centerY));

        // Directions: up, right, down, left
        int[] dx = { -1, 0, 1, 0 };
        int[] dy = { 0, 1, 0, -1 };

        while (queue.Count > 0)
        {
            var (x, y) = queue.Dequeue();

            for (int i = 0; i < 4; i++)
            {
                int nx = x + dx[i];
                int ny = y + dy[i];

                if (nx >= 0 && nx < MazeSize && ny >= 0 && ny < MazeSize)
                {
                    if ((MazeData[nx, ny] != Wall) && MazeDist[nx, ny] == -1)
                    {
                        MazeDist[nx, ny] = MazeDist[x, y] + 1;
                        queue.Enqueue((nx, ny));
                    }
                }
            }
        }
    }

    public void MakeMaze()
    {
        int ObjectiveCount = 0;
        for (int i = 0; i < MazeSize; i++)
        {
            for (int j = 0; j < MazeSize; j++)
            {
                int nx = i - MazeSize / 2;
                int nz = j - MazeSize / 2;

                if (MazeData[i, j] == Wall)
                {
                    StaticBody3D wall = WallScene.Instantiate<StaticBody3D>();
                    wall.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(wall);
                    MazeTiles[i, j] = wall;
                }
                else
                {

                    // Add floor and ceiling for lamp spaces too
                    StaticBody3D floor = FloorScene.Instantiate<StaticBody3D>();
                    floor.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(floor);
                    Paths.Add(floor);
                    MazeTiles[i, j] = floor;

                    if ((MazeDist[i, j] % LampSpacing == 0 && Lib.rand.Next(0, 2) == 0) || MazeDist[i, j] == 0)
                    {
                        Node3D lamp = LampScene.Instantiate<Node3D>();
                        lamp.Position = new Vector3(nx, 0, nz) * WallSize;
                        AddChild(lamp);
                        Lamps.Add(lamp);
                    }
                    if (MazeData[i, j] == ObjectiveLabel)
                    {
                        Objective objective = ObjectiveScene.Instantiate<Objective>();
                        objective.Position = new Vector3(nx, 0, nz) * WallSize;
                        ObjectiveCount++;
                        objective.PixelColor = ObjectiveColors[(ObjectiveCount - 1) % ObjectiveColors.Length];
                        Objectives.Add(objective);
                        //AddChild(objective);
                    }
                    if (MazeData[i, j] == EasterEgg)
                    {
                        EasterEggs = EasterScene.Instantiate<Easter>();
                        EasterEggs.Position = new Vector3(nx, 0, nz) * WallSize;
                        AddChild(EasterEggs);
                        PlaceFlashLight(EasterEggs.Position);

                        //EasterEggs.Add(easter);
                    }
                }
            }
        }

        HelperBody = HelperBodyScene.Instantiate<Node3D>();
        HelperBody.Position = new Vector3(0, 0, -WallSize * 2);
        AddChild(HelperBody);

    }
    public void PlaceFlashLight(Vector3 position)
    {
        // Calculate direction towards center (0,0,0)
        Vector3 directionToCenter = -position.Normalized();

        // Position flashlight one block closer to center
        Vector3 FlashLightPos = position + (directionToCenter * WallSize * new Vector3(1, 0, 1));

        FlashLight = FlashLightScene.Instantiate<FlashLight>();
        FlashLight.Position = FlashLightPos;
        AddChild(FlashLight);
    }

    public void DisplayMaze()
    {
        for (int i = 0; i < MazeSize; i++)
        {
            string line = "";
            for (int j = 0; j < MazeSize; j++)
            {
                switch (MazeData[i, j])
                {
                    case Wall:
                        line += "■ ";
                        break;
                    case Floor:
                        line += "  ";
                        break;
                    case Lamp:
                        line += "* ";
                        break;
                    case CantSpawn:
                        line += "X ";
                        break;
                    default:
                        line += MazeData[i, j] + " ";
                        break;
                }
            }
            //GD.Print(line);
        }
        //GD.Print("Maze size: " + MazeSize + "x" + MazeSize);

        for (int i = 0; i < MazeSize; i++)
        {
            string line = "";
            for (int j = 0; j < MazeSize; j++)
                line += MazeDist[i, j] + " ";
            //GD.Print(line);
        }


    }

    public void PlaceRoom(int x, int y, int w, int h, bool center = false, int Label = 0)
    {
        if (center)
        {
            x -= w / 2;
            y -= h / 2;
        }

        for (int i = x; i < x + w; i++)
        {
            for (int j = y; j < y + h; j++)
            {
                if (i > 0 && i < MazeSize - 1 && j > 0 && j < MazeSize - 1)
                {
                    MazeData[i, j] = Label;
                }
            }
        }
    }

    public void ReplaceLabel(int x, int y, int w, int h, int oldLabel, int newLabel, bool center = false)
    {
        if (center)
        {
            x -= w / 2;
            y -= h / 2;
        }

        for (int i = x; i < x + w; i++)
        {
            for (int j = y; j < y + h; j++)
            {
                if (i > 0 && i < MazeSize - 1 && j > 0 && j < MazeSize - 1)
                {
                    if (MazeData[i, j] == oldLabel)
                        MazeData[i, j] = newLabel;
                }
            }
        }
    }

    public void RemoveAllWall()
    {
        WallRemove = true;
        for (int i = 1; i < MazeSize - 1; i++)
        {
            for (int j = 1; j < MazeSize - 1; j++)
            {
                if (MazeData[i, j] == Wall)
                {
                    MazeData[i, j] = Floor;
                    WallSound.Play();
                    var tween = GetTree().CreateTween();
                    tween.TweenProperty(MazeTiles[i, j], "position", MazeTiles[i, j].Position + new Vector3(0, -2, 0), 1.8f);
                    tween.TweenCallback(Callable.From(() => MazeTiles[i, j].QueueFree()));

                    StaticBody3D floor = FloorScene.Instantiate<StaticBody3D>();
                    int nx = i - MazeSize / 2;
                    int nz = j - MazeSize / 2;
                    floor.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(floor);
                    MazeTiles[i, j] = floor;
                }
            }
        }

        foreach (var lamp in Lamps)
        {
            lamp.QueueFree();
        }
        Lamps.Clear();

        foreach (var enemy in Enemies)
        {
            if (enemy.IsInsideTree())
                enemy.QueueFree();
        }
        Enemies.Clear();

        Lamp Lamp = LampScene.Instantiate<Lamp>();
        Lamp.Position = new Vector3(0, 0, 0);
        Lamp.RenderDistance = 100;
        Lamp.Light.OmniRange = 30;
        Lamp.Light.LightIntensityLumens = 3000;
        Lamp.Light.LightEnergy = 100f;
        AddChild(Lamp);
        Lamp.timer.Stop();
        FinalLevel.Instance.Area3D.Position = new Vector3(0, 0, 0);
        RemoveChild(HelperBody);
        RemoveChild(EasterEggs);
        HelperBody.QueueFree();
        EasterEggs.QueueFree();

        if (FlashLight != null && FlashLight.IsInsideTree())
        {
            RemoveChild(FlashLight);
            FlashLight.QueueFree();
            FlashLight = null;
        }

        FinalLevel.Instance.Player3D.Flashlight.Visible = false;

    }
}
