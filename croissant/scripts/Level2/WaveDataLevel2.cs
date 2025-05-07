using Godot;
using System;
using System.Collections.Generic;

public partial class WaveDataLevel2 : WaveData
{

	public override void _Ready()
	{
		WaveStart = new Func<List<FloatWindow>>[NbOFWaves];
		WaveStart[0] = StartWave1;
		WaveStart[1] = StartWave2;
		WaveStart[2] = StartWave3;
		WaveStart[3] = StartWave4;
		WaveStart[4] = StartWave5;
		WaveStart[5] = StartWave6;
		WaveStart[6] = StartWave7;
		WaveStart[7] = StartWave8;
		WaveStart[8] = StartWave9;
		WaveStart[9] = StartWave10;

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public List<FloatWindow> StartWave1()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave2()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave3()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave4()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave5()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave6()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave7()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave8()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave9()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave10()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
		l.Random = false;
		windows.Add(l);

		ExtendWindow E = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
		E.Random = false;
		windows.Add(E);

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}
}