using Godot;
using System;

public partial class FinalLevel : Node3D
{
	[Export] public Maze maze;
	[Export] public MiniMap miniMap;
	[Export] public NavigationRegion3D NavigationRegion;
	[Export] public Player3D Player3D;
	public static FinalLevel Instance;
	[Export] public PackedScene Enemy3DScene;
	public int EnemyCount = 0;
	public override void _Ready()
	{
		Instance = this;
		maze.CreateMaze();
		//NavigationRegion.NavigationMesh = new NavigationMesh();
		NavigationRegion.BakeNavigationMesh();
		miniMap.DrawMaze();
		SpawnEnemy();

	}

	public void SpawnEnemy()
	{
		for (int i = 0; i < maze.MazeSize; i++)
		{
			for (int j = 0; j < maze.MazeSize; j++)
			{
				if (maze.MazeData[i, j] == 0 && Lib.rand.Next(0, 11) == 0)
				{
					Vector3 spawnPosition = ((new Vector3(i, 0, j) - new Vector3(maze.MazeSize / 2, 0, maze.MazeSize / 2)) * maze.WallSize) + new Vector3(maze.WallSize / 2, 0, maze.WallSize / 2);
					Enemy3D enemy = (Enemy3D)Enemy3DScene.Instantiate();
					enemy.Position = spawnPosition;
					AddChild(enemy);
					//enemy.GlobalPosition = spawnPosition;
					EnemyCount++;
				}
			}
		}
		Lib.Print("Enemy Count: " + FinalLevel.Instance.EnemyCount);
	}

	public override void _Process(double delta)
	{
	}
}