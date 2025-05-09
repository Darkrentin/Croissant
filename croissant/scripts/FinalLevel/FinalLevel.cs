using Godot;
using System;

public partial class FinalLevel : Node3D
{
	[Export] public Maze maze;
	[Export] public MiniMap miniMap;
	[Export] public NavigationRegion3D NavigationRegion;
	[Export] public Player3D Player3D;
	[Export] public PackedScene Enemy3DScene;
	[Export] public MeltShader MeltShader;
	public static FinalLevel Instance;
	public int ObjectiveDestroyed = 0;
	public int ObjectiveCount = 3;
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
			for (int j = 0; j < maze.MazeSize; j++)
				if (maze.MazeData[i, j] == 0 && Lib.rand.Next(0, 11) == 0)
				{
					Vector3 spawnPosition = ((new Vector3(i, 0, j) - new Vector3(maze.MazeSize / 2, 0, maze.MazeSize / 2)) * maze.WallSize) + new Vector3(maze.WallSize / 2, 0, maze.WallSize / 2);
					Enemy3D enemy = (Enemy3D)Enemy3DScene.Instantiate();
					enemy.Position = spawnPosition;
					AddChild(enemy);
					//enemy.GlobalPosition = spawnPosition;
					EnemyCount++;
				}
		Lib.Print("Enemy Count: " + Instance.EnemyCount);
	}

	public override void _Process(double delta)
	{

	}

	public void ObjectiveDestroy()
	{
		ObjectiveDestroyed++;
		if (ObjectiveDestroyed >= ObjectiveCount)
			Lib.Print("All objectives destroyed");
		else
			Lib.Print("Objective destroyed: " + ObjectiveDestroyed);
	}

	public void Death()
	{

		MeltShader.GenerateOffsets();
		
		FinalLevel.Instance.Player3D.GlobalPosition = new Vector3(0, 0, 0);
		FinalLevel.Instance.Player3D.ProcessMode = ProcessModeEnum.Disabled;
		MeltShader.Transition();
	}
}