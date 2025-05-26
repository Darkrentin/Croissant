using Godot;
using System;

public partial class BossLevel : Node3D
{
	// Called when the node enters the scene tree for the first time.

	public BossFloor[,] BossFloors;
	[Export] public PackedScene BossFloorScene;
	[Export] public PackedScene FloppyDiskScene;
	[Export] Area3D KillZone;
	[Export] Area3D NearZone;
	public int MapSize = 10;
	public const int WallSize = 2;
	public static BossLevel Instance;
	[Export] public Node3D[] SpawnPoints;
	[Export] public Virus3D Virus;
	public override void _Ready()
	{
		Instance = this;
		initMap();
		KillZone.BodyEntered += OnBodyEnteredKillZone;
		NearZone.BodyEntered += (body) => { VirusAttack(); };
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
	//Atk: MeleeAttack
	public void VirusAttack()
	{
		Virus.AnimationPlayer.Play("Atk");
	}
	//Atk: LiftWalls
	public void LiftWalls()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				if (Lib.rand.Next(0, 3) == 0 && !PlayerOnWall(BossFloors[i, j].GlobalPosition))
					BossFloors[i, j].PlayAnimation("Up", 0.1f, false, true);
			}
		}
	}

	public bool PlayerOnWall(Vector3 WallPosition)
	{
		Vector3 playerPos = FinalLevel.Instance.Player3D.GlobalPosition;

		// Calculate the distance between player and wall directly in world space
		float distanceX = Mathf.Abs(playerPos.X - WallPosition.X);
		float distanceZ = Mathf.Abs(playerPos.Z - WallPosition.Z);

		// Consider the player to be on the wall if they're within WallSize units
		// This covers the current wall and adjacent walls
		return distanceX <= WallSize * 1.5f && distanceZ <= WallSize * 1.5f;
	}
	//Atk: WaveAttack
	public void StartWave()
	{
		bool Wave = Lib.rand.Next(0, 2) == 0;
		LaunchWaves(Wave, !Wave);
	}

	// LaunchWaves method to create waves with appropriate safe zones
	public void LaunchWaves(bool launchXWave = true, bool launchZWave = true)
	{
		// Safe coordinates - only generate what's needed
		int? SafeX = launchXWave ? Lib.rand.Next(0, MapSize) : null;
		int? SafeY = launchZWave ? Lib.rand.Next(0, MapSize) : null;

		// Configure wave timing
		float time = 0.5f;
		float timeStep = 0.7f;

		// Animate both axes simultaneously with appropriate safe zones
		for (int i = 0; i < MapSize; i++)
		{
			// X-axis wave (columns)
			if (launchXWave)
			{
				for (int j = 0; j < MapSize; j++)
				{
					// Skip only the safe column if we're using X axis
					if (j == SafeX) continue;

					// Skip safe Y point only if Z axis is also active
					if (launchZWave && i == SafeY) continue;

					BossFloors[i, j].PlayAnimation("Lava", time);
				}
			}

			// Z-axis wave (rows)
			if (launchZWave)
			{
				for (int j = 0; j < MapSize; j++)
				{
					// Skip only the safe row if we're using Z axis
					if (j == SafeY) continue;

					// Skip safe X point only if X axis is also active
					if (launchXWave && i == SafeX) continue;

					BossFloors[j, i].PlayAnimation("Lava", time);
				}
			}

			// Increase time for the wave effect
			time += timeStep;
		}
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
	//Atk: LaunchFloppyDisk
	public void LaunchFloppyDisk()
	{
		int SpawnId = Lib.rand.Next(0, SpawnPoints.Length);
		Vector3 SpawnPosition = SpawnPoints[SpawnId].GlobalPosition;
		FloppyDisk floppyDisk = (FloppyDisk)FloppyDiskScene.Instantiate();
		AddChild(floppyDisk);
		floppyDisk.GlobalPosition = SpawnPosition;

	}
	
	public void LaunchNFloppyDisks(int n, float delayBetweenDisks = 0.0f)
	{
		for (int i = 0; i < n; i++)
		{
			if (i == 0 || delayBetweenDisks <= 0)
			{
				// Launch first disk immediately or if no delay is specified
				LaunchFloppyDisk();
			}
			else
			{
				// Launch subsequent disks with delay
				var timer = GetTree().CreateTimer(i * delayBetweenDisks);
				timer.Timeout += LaunchFloppyDisk;
			}
		}
	}
}
