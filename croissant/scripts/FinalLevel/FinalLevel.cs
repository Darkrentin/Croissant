using Godot;
using System;
using System.Collections.Generic;
using System.Data;

public partial class FinalLevel : Node3D
{
	[Export] public bool Debug = false;
	[Export] public Maze maze;
	[Export] public MiniMap miniMap;
	[Export] public NavigationRegion3D NavigationRegion;
	[Export] public Player3D Player3D;
	[Export] public PackedScene Enemy3DScene;
	[Export] public PackedScene BossLevelScene;
	[Export] public SubViewportContainer ShaderViewport;
	[Export] public MeltShader MeltShader;
	[Export] public CompressedTexture2D PaletteMain;
	[Export] public CompressedTexture2D PaletteDeath;
	[Export] public Area3D Area3D;
	[Export] public SubViewport GameNode;
	[Export] public StaticBody3D SafeZone;
	public Node3D BossLevel;
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

		foreach (Objective objective in maze.Objectives)
		{
			maze.AddChild(objective);
		}

		SafeZone.GetParent().RemoveChild(SafeZone);
		SafeZone.QueueFree();
		miniMap.DrawMaze();
		SpawnEnemy();
		Area3D.BodyEntered += EndReach;
		GameManager.MainWindow.AlwaysOnTop = true;
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
					maze.Enemies.Add(enemy);
					//enemy.GlobalPosition = spawnPosition;
					EnemyCount++;
				}
		Lib.Print("Enemy Count: " + Instance.EnemyCount);
	}

	public void ObjectiveDestroy()
	{
		ObjectiveDestroyed++;
		if (ObjectiveDestroyed >= ObjectiveCount)
		{
			Lib.Print("All objectives destroyed");
			maze.RemoveAllWall();
		}
		else
			Lib.Print("Objective destroyed: " + ObjectiveDestroyed);
	}

	public void Death(Vector3 enemyPosition)
	{
		Player3D.ProcessMode = ProcessModeEnum.Disabled;
		ShaderMaterial shaderMaterial = (ShaderMaterial)ShaderViewport.Material;
		shaderMaterial.SetShaderParameter("u_color_tex", PaletteDeath);

		Vector3 originalPlayerPos = Player3D.GlobalPosition;
		Vector3 direction = new Vector3(enemyPosition.X, originalPlayerPos.Y, enemyPosition.Z) - originalPlayerPos;
		float targetY = Mathf.Atan2(direction.X, direction.Z) + Mathf.Pi;
		while (targetY > Mathf.Pi) targetY -= Mathf.Pi * 2;
		while (targetY < -Mathf.Pi) targetY += Mathf.Pi * 2;

		var tween = CreateTween();
		tween.TweenProperty(Player3D, "rotation:y", targetY, 0.5f)
			 .SetTrans(Tween.TransitionType.Linear)
			 .SetEase(Tween.EaseType.InOut);
		tween.TweenInterval(0.5);
		tween.Finished += () =>
		{
			MeltShader.PrepareTransition();
			shaderMaterial.SetShaderParameter("u_color_tex", PaletteMain);
			MeltShader.Transition();
			Player3D.GlobalPosition = Vector3.Zero + new Vector3(1, 0, 1);
		};
	}

	public void EndReach(Node body)
	{
		if (body is Player3D player)
		{
			CallDeferred(nameof(TransitionToBossLevel));
		}
	}
	public void TransitionToBossLevel()
	{
		GameNode.RemoveChild(Area3D);
		GameNode.RemoveChild(NavigationRegion);
		NavigationRegion.QueueFree();
		BossLevel = BossLevelScene.Instantiate<Node3D>();
		GameNode.AddChild(BossLevel);
	}

	public override void _Process(double delta)
	{
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
	}
}