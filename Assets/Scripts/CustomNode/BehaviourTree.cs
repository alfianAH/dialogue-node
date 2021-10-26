using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;

    public Node.State Update()
    {
        return rootNode.Update();
    }
}