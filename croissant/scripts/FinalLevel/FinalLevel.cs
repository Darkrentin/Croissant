using Godot;
using System;

public partial class FinalLevel : Node3D
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Maze maze;
	[Export] public NavigationRegion3D NavigationRegion;
	[Export] public Player3D Player3D;
	public static FinalLevel Instance;
	public override void _Ready()
	{
		Instance = this;
		maze.CreateMaze();
		//NavigationRegion.NavigationMesh = new NavigationMesh();
		NavigationRegion.BakeNavigationMesh();

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
