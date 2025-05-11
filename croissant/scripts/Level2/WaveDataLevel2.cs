using Godot;
using System;
using System.Collections.Generic;

public partial class WaveDataLevel2 : WaveData
{

	public enum WindowType
	{
		Laser,
		Extend,
		Follow,
		Spike,
		Compress,
		Wave
	}

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

	public void AddWindow(List<FloatWindow> windows, WindowType type, bool random = false)
	{
		switch (type)
		{
			case WindowType.Laser:
				LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
				l.Random = random;
				windows.Add(l);
				break;
			case WindowType.Extend:
				ExtendWindow e = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
				e.Random = random;
				windows.Add(e);
				break;
			case WindowType.Follow:
				FollowWindow f = States.SceneLoader.FollowWindowScene.Instantiate<FollowWindow>();
				f.Random = random;
				windows.Add(f);
				break;
			case WindowType.Spike:
				SpikeWindow s = States.SceneLoader.SpikeWindowScene.Instantiate<SpikeWindow>();
				s.Random = random;
				windows.Add(s);
				break;
			case WindowType.Compress:
				CompressWindow c = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
				c.Random = random;
				windows.Add(c);
				break;
			case WindowType.Wave:
				WaveWindow w = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
				w.Random = random;
				windows.Add(w);
				break;
		}
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

		return windows;
	}

	public List<FloatWindow> StartWave2()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		AddWindow(windows, WindowType.Laser, false);

		return windows;
	}

	public List<FloatWindow> StartWave3()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		FollowWindow F = States.SceneLoader.FollowWindowScene.Instantiate<FollowWindow>();
		F.Random = false;
		windows.Add(F);

		return windows;
	}

	public List<FloatWindow> StartWave4()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		SpikeWindow S = States.SceneLoader.SpikeWindowScene.Instantiate<SpikeWindow>();
		S.Random = false;
		windows.Add(S);

		return windows;
	}

	public List<FloatWindow> StartWave5()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		CompressWindow C = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
		C.Random = false;
		windows.Add(C);

		return windows;
	}

	public List<FloatWindow> StartWave6()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

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