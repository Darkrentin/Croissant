using Godot;
using System;
using System.Collections.Generic;

public partial class WaveData : Node
{
	public Func<List<FloatWindow>>[] WaveStart;
	[Export] public int NbOFWaves;
}
