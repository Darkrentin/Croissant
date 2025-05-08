using Godot;
using System;

public partial class MiniMap : TileMapLayer
{
	// Called when the node enters the scene tree for the first time.
	[Export] public Maze Maze;

	[Export] public Sprite2D Sprite;
    [Export] public CharacterBody3D Player;
	Vector2I atlasCoordinates = new Vector2I(0, 0);

    public int sourceId = 0;
    public int layer = 0;
	public override void _Ready()
	{
		sourceId = TileSet.GetSourceId(0);
        layer = 0;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Sprite.Position = (new Vector2(Player.GlobalPosition.X/2, Player.GlobalPosition.Z/2) + new Vector2(Maze.MazeSize / 2, Maze.MazeSize / 2)) * 16;

	}

	public void DrawMaze()
	{
		for(int i = 0; i < Maze.MazeSize; i++)
		{
			for(int j = 0; j < Maze.MazeSize; j++)
			{
				if(Maze.MazeData[i,j] == 1)
                {
                    SetCell(new Vector2I(i,j), sourceId, atlasCoordinates);
                }
			}
		}
	}
}
