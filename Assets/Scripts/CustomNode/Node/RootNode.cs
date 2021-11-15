using UnityEngine;

public class RootNode: Node
{
    public Node child;

    protected override void OnStart()
    {
        Debug.Log("Root node: Start");
    }

    protected override void OnStop()
    {
        Debug.Log("Root node: Stop");
    }

    protected override State OnUpdate()
    {
        Debug.Log("Root node: Update");
        return child.Update();
    }
    
    public override Node Clone()
    {
        RootNode node = Instantiate(this);
        node.child = child.Clone();
        return node;
    }
}