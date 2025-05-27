using Godot;
using System;

public partial class BossLevel : Node3D
{
	// Called when the node enters the scene tree for the first time.

	public BossFloor[,] BossFloors;
	[Export] public PackedScene BossFloorScene;
	[Export] public PackedScene FloppyDiskScene;
	[Export] Area3D KillZone;
	public int MapSize = 12;
	public const int WallSize = 2;
	public static BossLevel Instance;
	[Export] public Node3D SpawnPoints;
	[Export] public Virus3D Virus;
	public override void _Ready()
	{
		Instance = this;
		initMap();
		KillZone.BodyEntered += OnBodyEnteredKillZone;
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
	//Atk: FloorAttack
	public void FloorAttack()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				if (Lib.rand.Next(0, 3) == 0)
					BossFloors[i, j].animationPlayer.Play("Lava");
			}
		}
	}

	//Atk: LiftWalls
	public void LiftWalls()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				if (Lib.rand.Next(0, 3) == 0 && !PlayerOnWall(BossFloors[i, j].GlobalPosition))
					BossFloors[i, j].animationStateMachine.Travel("Up");
			}
		}
		GetTree().CreateTimer(3f).Timeout += () =>
		{
			IdleMap();
		};
	}

	public bool PlayerOnWall(Vector3 WallPosition)
	{
		Vector3 playerPos = FinalLevel.Instance.Player3D.GlobalPosition;

		// Calculate the distance between player and wall directly in world space
		float distanceX = Mathf.Abs(playerPos.X - WallPosition.X - 1);
		float distanceZ = Mathf.Abs(playerPos.Z - WallPosition.Z - 1);

		// Consider the player to be on the wall if they're within WallSize units
		// This covers the current wall and adjacent walls
		return distanceX <= WallSize * 1f && distanceZ <= WallSize * 1f;
	}
	//Atk: Lava
	public void Lava()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				if (!PlayerOnWall(BossFloors[i, j].GlobalPosition))
					BossFloors[i, j].animationStateMachine.Travel("Lava");
			}
		}
		GetTree().CreateTimer(5f).Timeout += () =>
		{
			IdleMap();
		};
	}


	public void OnBodyEnteredKillZone(Node3D body)
	{
		FinalLevel.Instance.DeathBossLevel();
	}

	public void ResetMap()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				BossFloors[i, j].animationStateMachine.Travel("RESET");
				BossFloors[i, j].timer.Stop();
			}
		}
	}
	public void IdleMap()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				BossFloors[i, j].animationStateMachine.Travel("Idle");
				BossFloors[i, j].timer.Stop();
			}
		}
	}
	//Atk: LaunchFloppyDisk
	public void LaunchFloppyDisk()
	{
		Vector3 SpawnPosition = SpawnPoints.GlobalPosition;
		FloppyDisk floppyDisk = (FloppyDisk)FloppyDiskScene.Instantiate();
		AddChild(floppyDisk);
		floppyDisk.GlobalPosition = SpawnPosition;

	}

	public void LaunchNFloppyDisk(int n, float delay = 0.3f)
	{
		for (int i = 0; i < n; i++)
		{
			GetTree().CreateTimer(i * delay).Timeout += () =>
			{
				LaunchFloppyDisk();
			};
		}
	}

	public void LaunchNFloppyDiskAndRotate(int n, float angle, float RotationSpeed, float delay = 0.3f)
	{
		for (int i = 0; i < n; i++)
		{
			GetTree().CreateTimer(i * delay).Timeout += () =>
			{
				LaunchFloppyDisk();
			};
		}
		Virus3D.Instance.Rotate(angle, RotationSpeed);
	}
}
