using UnityEngine;

public abstract class Node : ScriptableObject
{
    public enum State
    {
        Running, Failure, Success
    }

    public State state = State.Running;
    public bool started = false;
    public string guid;
    public Vector2 position;

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

    protected abstract void OnStart();
    protected abstract void OnStop();
    protected abstract State OnUpdate();
}
