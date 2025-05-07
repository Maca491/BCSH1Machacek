using UnityEngine;

public abstract class State<T>
{
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
}