using Godot;
using System;

public partial class BossLevel : Node3D
{
	// Called when the node enters the scene tree for the first time.

	public BossFloor[,] BossFloors;
	[Export] public PackedScene BossFloorScene;
	public int MapSize = 10;
	public const int WallSize = 2;
	public static BossLevel Instance;
	public override void _Ready()
	{
		Instance = this;
		initMap();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void initMap()
	{
		BossFloors = new BossFloor[MapSize, MapSize];
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				BossFloors[i, j] = BossFloorScene.Instantiate<BossFloor>();
				BossFloors[i, j].Position = GetPosition(i, j);
				AddChild(BossFloors[i, j]);
			}
		}
	}

	public Vector3 GetPosition(int i, int j)
	{
		return new Vector3(i - MapSize / 2, 0, j - MapSize / 2) * WallSize;
	}

	//Attack

	public void FloorAttack()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				if(Lib.rand.Next(0, 3) == 0)
					BossFloors[i, j].animationPlayer.Play("Lava");
			}
		}
	}
}
