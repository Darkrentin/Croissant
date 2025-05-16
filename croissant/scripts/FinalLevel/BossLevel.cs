using Godot;
using System;

public partial class BossLevel : Node3D
{
	// Called when the node enters the scene tree for the first time.

	public BossFloor[,] BossFloors;
	[Export] public PackedScene BossFloorScene;
	[Export] Area3D KillZone;
	public int MapSize = 10;
	public const int WallSize = 2;
	public static BossLevel Instance;
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
		FinalLevel.Instance.Death(Vector3.Zero);
	}

	public void ResetMap()
	{
		for (int i = 0; i < MapSize; i++)
		{
			for (int j = 0; j < MapSize; j++)
			{
				BossFloors[i, j].animationPlayer.Play("RESET");
				BossFloors[i, j].timer.Stop();
			}
		}
	}
}
