using Godot;
using System;
using System.Collections.Generic;

public partial class WaveDataLevel2 : WaveData
{

	public enum WindowType { Laser, Extend, Follow, Spike, Compress, Wave }

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
		WaveStart[10] = StartWave11;
		WaveStart[11] = StartWave12;
		WaveStart[12] = StartWave13;
		WaveStart[13] = StartWave14;
		WaveStart[14] = StartWave15;
		WaveStart[15] = StartWave16;
		WaveStart[16] = StartWave17;
		WaveStart[17] = StartWave18;
		WaveStart[18] = StartWave19;
		WaveStart[19] = StartWave20;
		WaveStart[20] = StartWave21;
		WaveStart[21] = StartWave22;
		WaveStart[22] = StartWave23;
		WaveStart[23] = StartWave24;
	}

	public void AddWindow(List<FloatWindow> windows, WindowType type, bool random = false)
	{
		switch (type)
		{
			case WindowType.Laser:
				LaserWindow l = States.SceneLoader.LaserWindowScene.Instantiate<LaserWindow>();
				l.RandomPosition = random;
				windows.Add(l);
				break;
			case WindowType.Extend:
				ExtendWindow e = States.SceneLoader.ExtendWindowScene.Instantiate<ExtendWindow>();
				e.RandomPosition = random;
				windows.Add(e);
				break;
			case WindowType.Follow:
				FollowWindow f = States.SceneLoader.FollowWindowScene.Instantiate<FollowWindow>();
				f.RandomPosition = random;
				windows.Add(f);
				break;
			case WindowType.Spike:
				SpikeWindow s = States.SceneLoader.SpikeWindowScene.Instantiate<SpikeWindow>();
				s.RandomPosition = random;
				windows.Add(s);
				break;
			case WindowType.Compress:
				CompressWindow c = States.SceneLoader.CompressWindowScene.Instantiate<CompressWindow>();
				c.RandomPosition = random;
				windows.Add(c);
				break;
			case WindowType.Wave:
				WaveWindow w = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
				w.RandomPosition = random;
				windows.Add(w);
				break;
		}
	}

	public List<FloatWindow> StartWave1()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Spike, true);
		AddWindow(windows, WindowType.Spike, false);
		AddWindow(windows, WindowType.Laser, true);
		return windows;
	}

	public List<FloatWindow> StartWave2()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, false);
		return windows;
	}

	public List<FloatWindow> StartWave3()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Extend, true);
		AddWindow(windows, WindowType.Extend, false);
		AddWindow(windows, WindowType.Spike, true);
		return windows;
	}

	public List<FloatWindow> StartWave4()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Compress, true);
		AddWindow(windows, WindowType.Compress, false);
		AddWindow(windows, WindowType.Spike, false);
		return windows;
	}

	public List<FloatWindow> StartWave5()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F._Mode = 1;
		F.RandomPosition = true;
		windows.Add(F);
		AddWindow(windows, WindowType.Follow, true);
		AddWindow(windows, WindowType.Follow, false);
		return windows;
	}

	public List<FloatWindow> StartWave6()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		return windows;
	}

	public List<FloatWindow> StartWave7()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Spike, true);
		WaveWindow F = States.SceneLoader.FlappyWindowScene.Instantiate<WaveWindow>();
		F._Mode = 2;
		F.RandomPosition = true;
		windows.Add(F);
		AddWindow(windows, WindowType.Extend, false);
		AddWindow(windows, WindowType.Extend, false);
		return windows;
	}

	public List<FloatWindow> StartWave8()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Spike, true);
		AddWindow(windows, WindowType.Follow, false);
		AddWindow(windows, WindowType.Follow, false);
		AddWindow(windows, WindowType.Follow, false);
		AddWindow(windows, WindowType.Follow, false);
		return windows;
	}

	public List<FloatWindow> StartWave9()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Spike, true);
		AddWindow(windows, WindowType.Compress, true);
		AddWindow(windows, WindowType.Compress, false);
		AddWindow(windows, WindowType.Compress, true);
		return windows;
	}

	public List<FloatWindow> StartWave10()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Spike, true);
		AddWindow(windows, WindowType.Compress, false);
		AddWindow(windows, WindowType.Extend, true);
		AddWindow(windows, WindowType.Follow, true);
		return windows;
	}

	public List<FloatWindow> StartWave11()
	{
		List<FloatWindow> windows = new List<FloatWindow>();
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, true);
		AddWindow(windows, WindowType.Laser, true);
		return windows;
	}

	public List<FloatWindow> StartWave12()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave13()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave14()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave15()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave16()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave17()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave18()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave19()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave20()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave21()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave22()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave23()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}

	public List<FloatWindow> StartWave24()
	{
		List<FloatWindow> windows = new List<FloatWindow>();

		return windows;
	}
}