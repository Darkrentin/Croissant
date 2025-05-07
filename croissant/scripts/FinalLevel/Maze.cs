using Godot;
using System;

public partial class Maze : Node3D
{
    // Called when the node enters the scene tree for the first time.

    public int MazeSize = 11; // Pour un labyrinthe intéressant avec cette méthode, préférez une taille impaire >= 5 si vous voulez des bordures et des piliers clairs. Ex: 11, 13.
    public int[,] MazeData;
	[Export] public PackedScene WallScene;
	[Export] public PackedScene LampScene;

    public override void _Ready()
    {
        initMaze();
        DisplayMaze();
		MakeMaze();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void initMaze()
    {
        MazeData = new int[MazeSize, MazeSize];

        for (int i = 0; i < MazeSize; i++)
        {
            for (int j = 0; j < MazeSize; j++)
            {
                MazeData[i, j] = 1; // Initialise tout en mur
            }
        }

        if (MazeSize > 0)
        {
            GenerateMazeDFS(1, 1);
        }
		MazeData[MazeSize/2, MazeSize/2] = 0; // Sortie
    }
    private void GenerateMazeDFS(int r, int c)
    {

        MazeData[r, c] = 0;

        int[] directions = { 0, 1, 2, 3 }; // 0:N, 1:E, 2:S, 3:W
        Lib.rand.Shuffle(directions); 

        foreach (int dir in directions)
        {
            int nextR = r;
            int nextC = c; 
            int wallR = r; 
            int wallC = c; 

            switch (dir)
            {
                case 0: 
                    nextR -= 2; wallR -= 1;
                    break;
                case 1: 
                    nextC += 2; wallC += 1;
                    break;
                case 2: 
                    nextR += 2; wallR += 1;
                    break;
                case 3: 
                    nextC -= 2; wallC -= 1;
                    break;
            }

            if (nextR >= 0 && nextR < MazeSize && nextC >= 0 && nextC < MazeSize)
            {

                if (MazeData[nextR, nextC] == 1)
                {
                    MazeData[wallR, wallC] = 0; 
                    GenerateMazeDFS(nextR, nextC); 
                }
            }
        }
    }

	public void MakeMaze()
	{
		const int WallSize = 2;
		for(int i = 0; i < MazeSize; i++)
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
				if(MazeData[i, j] == 0 && i%2!=0 && j%2!=0)
				{
					int nx = i - MazeSize / 2;
					int nz = j - MazeSize / 2;
					Node3D lamp = LampScene.Instantiate<Node3D>();
					lamp.Position = new Vector3(nx, 0, nz) * WallSize;
					AddChild(lamp);
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

                line += (MazeData[i, j] == 1 ? "#" : ".") + " "; 
            }
            GD.Print(line);
        }
    }
}
