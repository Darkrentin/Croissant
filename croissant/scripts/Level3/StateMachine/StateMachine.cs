using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class StateMachine : Node
{
    private Dictionary<string, State> _states = new();
    private State _currentState;

    [Export]
    public State InitialState;

    public override void _Ready()
    {
        foreach (Node child in GetChildren())
        {
            if (child is State state)
            {
                _states[state.Name.ToString().ToLower()] = state;
                state.StateTransition += ChangeState;
            }
        }

        if (InitialState != null)
        {
            InitialState.Enter();
            _currentState = InitialState;
        }
    }


    public override void _Process(double delta)
    {
        _currentState?.Update((float)delta);
    }

    private void ChangeState(State sourceState, string newStateName)
    {
        if (sourceState != _currentState)
        {
            GD.Print($"Invalid change_state attempt from: {sourceState.Name} but currently in: {_currentState?.Name}");
            return;
        }

        if (!_states.TryGetValue(newStateName.ToLower(), out State newState))
        {
            GD.Print("New state is empty");
            return;
        }

        _currentState?.Exit();
        newState.Enter();
        _currentState = newState;
    }
}
