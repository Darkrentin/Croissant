using Godot;

public partial class FinalLevel : Node3D
{
	[Export] public bool Debug = false;
	[Export] public Maze maze;
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
	[Export] public AnimationPlayer AnimationPlayer;
	[Export] public AudioStreamPlayer DeathSound;
	[Export] public AudioStreamPlayer TransitionSound;
	[Export] public AudioStreamPlayer DeathLavaSound;
	[Export] public AudioStreamPlayer LavaSound;
	[Export] public AudioStreamPlayer WallSound;
	[Export] public Label ObjectiveLabel;
	public Node3D BossLevel;
	public static FinalLevel Instance;
	public int ObjectiveDestroyed = 0;
	public int ObjectiveCount = 3;
	public int EnemyCount = 0;
	public Timer DeathTimer;
	public ShaderMaterial shaderMaterial;
	[Export] public bool EndButton { get => false; set { if (value) End(); } }
	public override void _Ready()
	{
		GameManager.MainWindow.ContentScaleMode = Window.ContentScaleModeEnum.CanvasItems;
		GameManager.MainWindow.ContentScaleAspect = Window.ContentScaleAspectEnum.Ignore;
		Instance = this;
		maze.CreateMaze();
		//NavigationRegion.NavigationMesh = new NavigationMesh();
		NavigationRegion.BakeNavigationMesh();

		foreach (Objective objective in maze.Objectives)
			maze.AddChild(objective);

		SafeZone.GetParent().RemoveChild(SafeZone);
		SafeZone.QueueFree();
		SpawnEnemy();
		Area3D.BodyEntered += EndReach;

		DeathTimer = new Timer();
		DeathTimer.OneShot = true;
		AddChild(DeathTimer);
		DeathTimer.Timeout += () =>
		{
			MeltShader.PrepareTransition();
			shaderMaterial.SetShaderParameter("u_color_tex", PaletteMain);
			MeltShader.Transition();
			Player3D.GlobalPosition = Vector3.Zero + new Vector3(1, 0, 1);
			(BossLevel as BossLevel).ResetMap();
			Player3D.Alive = true;
			return;
		};

		shaderMaterial = (ShaderMaterial)ShaderViewport.Material;
		GameManager.MainWindow.GrabFocus();
		GameManager.MainWindow.Show();
		GameManager.MainWindow.SetProcessInput(true);

	}

	public void SpawnEnemy()
	{
		for (int i = 0; i < maze.MazeSize; i++)
			for (int j = 0; j < maze.MazeSize; j++)
				if (maze.MazeData[i, j] == 0 && Lib.rand.Next(0, 16) == 0)
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
			TransitionSound.Play();
			Player3D.DrawFootsteps = false;
		}
		else
			Lib.Print("Objective destroyed: " + ObjectiveDestroyed);
	}

	public void DeathBossLevel()
	{
		Death(Vector3.Zero);
	}

	public void Death(Vector3 enemyPosition)
	{
		shaderMaterial.SetShaderParameter("u_color_tex", PaletteDeath);
		Player3D.Alive = false;


		if (enemyPosition == Vector3.Zero)
		{
			Virus3D.Instance.HealVirus();
			DeathTimer.WaitTime = 1f;
			DeathTimer.Start();
			return;
		}

		Vector3 originalPlayerPos = Player3D.GlobalPosition;
		Vector3 direction = new Vector3(enemyPosition.X, originalPlayerPos.Y, enemyPosition.Z) - originalPlayerPos;
		float targetY = Mathf.Atan2(direction.X, direction.Z) + Mathf.Pi;
		while (targetY > Mathf.Pi) targetY -= Mathf.Pi * 2;
		while (targetY < -Mathf.Pi) targetY += Mathf.Pi * 2;


		var tween = CreateTween();
		tween.TweenProperty(Player3D, "rotation:y", targetY, 1f)
			 .SetTrans(Tween.TransitionType.Linear)
			 .SetEase(Tween.EaseType.InOut);
		//tween.TweenInterval(0.5f);
		tween.Finished += () =>
		{
			MeltShader.PrepareTransition();
			shaderMaterial.SetShaderParameter("u_color_tex", PaletteMain);
			MeltShader.Transition();
			DeathSound.Play();
			Player3D.GlobalPosition = Vector3.Zero + new Vector3(1, 0, 1);
			Player3D.Alive = true;
		};
	}

	public void EndReach(Node body)
	{
		if (body is Player3D player)
			CallDeferred(nameof(TransitionToEnd));
	}
	public void TransitionToBossLevel()
	{
		if (!maze.WallRemove)
			maze.RemoveAllWall();
		GameManager.PlayMusic(GameManager.Music.FinalBoss);
		GameNode.RemoveChild(Area3D);
		GameNode.RemoveChild(NavigationRegion);
		NavigationRegion.QueueFree();
		BossLevel = BossLevelScene.Instantiate<Node3D>();
		GameNode.AddChild(BossLevel);
		GameManager.SkipLevel = Virus3D.Instance.EndActions;
		Player3D.GlobalPosition = Vector3.Zero + new Vector3(1, 0, 1);
	}

	public void TransitionToEnd()
	{
		FinalLevel.Instance.AnimationPlayer.Play("BossDeath");
		GameManager.Instance.BossExplosionSound.Play();
		FinalLevel.Instance.Player3D.ProcessMode = ProcessModeEnum.Disabled;
		SpeedRunTimer.Instance.StopTimer();
		GameManager.HaveFinishTheGameAtLeastOneTime = true;
		GameManager.SaveData.Save();
	}

	public override void _Process(double delta)
	{

		// Your existing code
		if (Input.MouseMode == Input.MouseModeEnum.Visible)
			Input.MouseMode = Input.MouseModeEnum.Captured;
	}

	public void End()
	{
		GameManager.State = GameManager.GameState.Scoreboard;
		GetParent().RemoveChild(this);

		GameManager.MainWindow.AlwaysOnTop = false;
		Input.MouseMode = Input.MouseModeEnum.Visible;

		QueueFree();
	}

	public void ObjectiveFind()
	{
		ObjectiveLabel.Text = $"{ObjectiveDestroyed}/{ObjectiveCount}";
		AnimationPlayer.Play("ObjectiveFind");
	}
}
