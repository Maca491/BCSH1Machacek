using System;
using UnityEditor.Rendering;

public class StateMachine<T>
{
    private State<T> currentState;
    private State<T> previousState;

    public void Initialize(State<T> startingState)
    {
        previousState = null;
        currentState = startingState;
        currentState.Enter();
    }

    public void ChangeState(State<T> newState)
    {
        previousState = currentState;
        currentState?.Exit();
        currentState = newState;
        currentState.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }

    public void ReturnToPreviousState()
    {
        if (previousState != null)
        {
            ChangeState(previousState);
        }
    }

    public bool IsInState<U>() where U : State<T>
    {
        return currentState is U;
    }

    public State<T> GetCurrentState()
    {
        return currentState;
    }

    public State<T> GetPreviousState()
    {
        return previousState;
    }
}
