using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running, Failure, Success
    }

    [HideInInspector] public State state = State.Running;
    [HideInInspector] public bool started = false;
    [HideInInspector] public string guid;
    [HideInInspector] public Vector2 position;
    [HideInInspector] public Blackboard blackboard;
    [TextArea] public string description;

    public State Update()
    {
        // If not started yet, ...
        if (!started)
        {
            // Start it
            OnStart();
            started = true;
        }
        
        state = OnUpdate(); // Update the state
        
        // If the state is failure or success, ...
        if (state == State.Failure || state == State.Success)
        {
            // Stop it
            OnStop();
            started = false;
        }

        return state;
    }

    public virtual Node Clone()
    {
        return Instantiate(this);
    }

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
