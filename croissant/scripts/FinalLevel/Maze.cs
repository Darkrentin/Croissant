using Godot;

public partial class Maze : Node3D
{
    [Export] public PackedScene WallScene;
    [Export] public PackedScene LampScene;
    [Export] public int MazeSize = 31;

    public int WallSize = 2;
    public const int LampSpacing = 10;

    public const int Lamp = -2;
    public const int CantSpawn = -1;
    public const int Floor = 0;
    public const int Wall = 1;
    
    public int[,] MazeData;
    public int[,] MazeDist;
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

    public override void _Process(double delta)
    {
    }

    public void initMaze()
    {
        MazeData = new int[MazeSize, MazeSize];
        MazeDist = new int[MazeSize, MazeSize];

        for (int i = 0; i < MazeSize; i++)
            for (int j = 0; j < MazeSize; j++)
            {
                MazeData[i, j] = Wall;
                MazeDist[i, j] = -1;
            }

        if (MazeSize > 2)
            GenerateMazeDFS(1, 1, 0);

        PlaceRoom(MazeSize / 2, MazeSize / 2, 3, 3, true);
        const int SafeZone = 11;
        ReplaceLabel(MazeSize / 2, MazeSize / 2, SafeZone, SafeZone, Floor, CantSpawn, true);
        PlaceRoom(2, 2, 3, 3, true, CantSpawn);
        PlaceRoom(2, MazeSize - 3, 3, 3, true, CantSpawn);
        PlaceRoom(MazeSize - 3, 2, 3, 3, true, CantSpawn);
        PlaceRoom(MazeSize - 3, MazeSize - 3, 3, 3, true, CantSpawn);
    }

    private void GenerateMazeDFS(int r, int c, int depth)
    {
        MazeData[r, c] = Floor;
        MazeDist[r, c] = depth;
        depth++;

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
                    MazeDist[wallR, wallC] = depth;

                    GenerateMazeDFS(nextR, nextC, depth+1);
                }
                else if (MazeData[wallR, wallC] == Wall &&
                         nextR > 0 && nextR < MazeSize - 1 && nextC > 0 && nextC < MazeSize - 1 &&
                         (MazeData[nextR, nextC] == Floor))
                {
                    if (Lib.rand.NextDouble() < LoopCreationProbability)
                    {
                        MazeData[wallR, wallC] = Floor;
                        MazeDist[wallR, wallC] = depth;
                    }
                }
            }
        }
    }

    public void MakeMaze()
    {
        for (int i = 0; i < MazeSize; i++)
        {
            for (int j = 0; j < MazeSize; j++)
            {
                int nx = i - MazeSize / 2;
                int nz = j - MazeSize / 2;
                
                if (MazeData[i, j] == Wall)
                {
                    Node3D wall = WallScene.Instantiate<Node3D>();
                    wall.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(wall);
                }
                else
                {
                    
                    // Add floor and ceiling for lamp spaces too
                    Node3D floor = WallScene.Instantiate<Node3D>();
                    floor.Position = new Vector3(nx, 0, nz) * WallSize;
                    floor.Position += new Vector3(0, -WallSize, 0);
                    AddChild(floor);

                    Node3D ceil = WallScene.Instantiate<Node3D>();
                    ceil.RemoveChild(ceil.GetNode("CollisionShape3D"));
                    ceil.Position = new Vector3(nx, 0, nz) * WallSize;
                    ceil.Position += new Vector3(0, WallSize * 1, 0);
                    AddChild(ceil);

                    if(MazeDist[i,j]%LampSpacing==0)
                    {
                        Node3D lamp = LampScene.Instantiate<Node3D>();
                        lamp.Position = new Vector3(nx, 0, nz) * WallSize;
                        AddChild(lamp);
                    }
                }
            }
        }
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
            GD.Print(line);
        }
        GD.Print("Maze size: " + MazeSize + "x" + MazeSize);

        
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
}