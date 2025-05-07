using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class Maze : Node3D
{
    [Export] public PackedScene WallScene;
    [Export] public PackedScene LampScene;
    [Export] public int MazeSize = 21;
    
    public int[,] MazeData;
    [Export(PropertyHint.Range, "0.0,1.0,0.01")] public float LoopCreationProbability = 0.1f;   

    public override void _Ready()
    {
        if (MazeSize % 2 == 0)
        {
            MazeSize++;
        }
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

        for (int i = 0; i < MazeSize; i++)
            for (int j = 0; j < MazeSize; j++)
                MazeData[i, j] = 1;

        if (MazeSize > 2)
            GenerateMazeDFS(1, 1);
    }

    private void GenerateMazeDFS(int r, int c)
    {
        MazeData[r, c] = 0;
        if (Lib.rand.Next(0, 2) == 0)
            MazeData[r, c] = 2;

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
                    MazeData[nextR, nextC] == 1)
                {
                    MazeData[wallR, wallC] = 0;
                    GenerateMazeDFS(nextR, nextC);
                }
                else if (MazeData[wallR, wallC] == 1 &&
                         nextR > 0 && nextR < MazeSize - 1 && nextC > 0 && nextC < MazeSize - 1 &&
                         (MazeData[nextR, nextC] == 0 || MazeData[nextR, nextC] == 2))
                {
                    if (Lib.rand.NextDouble() < LoopCreationProbability)
                    {
                        MazeData[wallR, wallC] = 0;
                    }
                }
            }
        }
    }

    public void MakeMaze()
    {
        const int WallSize = 2;
        for (int i = 0; i < MazeSize; i++)
        {
            for (int j = 0; j < MazeSize; j++)
            {
                if (MazeData[i, j] == 1)
                {
                    int nx = i - MazeSize / 2;
                    int nz = j - MazeSize / 2;
                    Node3D wall = WallScene.Instantiate<Node3D>();
                    wall.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(wall);
                }
                else if (MazeData[i, j] == 2)
                {
                    int nx = i - MazeSize / 2;
                    int nz = j - MazeSize / 2;
                    Node3D lamp = LampScene.Instantiate<Node3D>();
                    lamp.Position = new Vector3(nx, 0, nz) * WallSize;
                    AddChild(lamp);
                }

                if (MazeData[i, j] == 0 || MazeData[i, j] == 2)
                {
                    int nx = i - MazeSize / 2;
                    int nz = j - MazeSize / 2;
                    Node3D floor = WallScene.Instantiate<Node3D>();
                    floor.Position = new Vector3(nx, 0, nz) * WallSize;
                    floor.Position += new Vector3(0, -WallSize, 0);
                    AddChild(floor);

                    Node3D ceil = WallScene.Instantiate<Node3D>();
                    ceil.Position = new Vector3(nx, 0, nz) * WallSize;
                    ceil.Position += new Vector3(0, WallSize * 1, 0);
                    AddChild(ceil);
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
                line += (MazeData[i, j] == 1 ? "#" : (MazeData[i,j] == 2 ? "L" : ".")) + " ";
                
                
            }
            GD.Print(line);
        }
    }
}
