using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    public Node.State Update()
    {
        // If root node's state is running, ...
        if(rootNode.state == Node.State.Running)
        {
            // Update the tree state
            treeState = rootNode.Update();
        }

        return treeState;
    }
}