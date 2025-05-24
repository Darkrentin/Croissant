using Godot;
using System;

public partial class PlayerStates : Node
{
    [Export] public Node Locked { get; private set; }
    [Export] public Node Idle { get; private set; }
    [Export] public Node Run { get; private set; }
    [Export] public Node Jump { get; private set; }
    [Export] public Node JumpPeak { get; private set; }
    [Export] public Node WallJump { get; private set; }
    [Export] public Node Fall { get; private set; }
}
