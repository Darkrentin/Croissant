using Godot;
using System;

public partial class State : Node
{
    [Signal]
    public delegate void StateTransitionEventHandler(State from, string to);

    public virtual void Enter() {}
    public virtual void Exit() {}
    public virtual void Update(float delta) {}
}
