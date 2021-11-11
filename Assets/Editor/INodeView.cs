using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public abstract class INodeView: UnityEditor.Experimental.GraphView.Node
{
    protected INodeView(string uiFile) : base(uiFile) { }
    public Action<INodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    // public List<Port> Outputs = new List<Port>();
    
    /// <summary>
    /// Update node's class according to its state
    /// </summary>
    public void UpdateState()
    {
        RemoveFromClassList("running");
        RemoveFromClassList("failure");
        RemoveFromClassList("success");
        
        // If is in play mode, ...
        if(Application.isPlaying)
        {
            switch (node.state)
            {
                case Node.State.Running:
                    // If node is already started
                    // Because the default state is running
                    if(node.started)
                        AddToClassList("running");
                    break;
                case Node.State.Failure:
                    AddToClassList("failure");
                    break;
                case Node.State.Success:
                    AddToClassList("success");
                    break;
            }
        }
    }
    
    /// <summary>
    /// Sort nodes by vertical position 
    /// </summary>
    /// <param name="above">Above node</param>
    /// <param name="below">Below node</param>
    /// <returns>Returns -1 if above node's y  position less than below one, else 1</returns>
    protected int SortByVerticalPosition(Node above, Node below)
    {
        return above.position.y < below.position.y ? -1 : 1;
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        // Record undo
        Undo.RecordObject(node, "Behaviour Tree (Set Position)");
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
        EditorUtility.SetDirty(node);
    }
    
    /// <summary>
    /// Set up class to each node to color it
    /// </summary>
    protected abstract void SetupClasses();
    
    /// <summary>
    /// Create input port in node
    /// </summary>
    protected abstract void CreateInputPorts();
    
    /// <summary>
    /// Sort composite node children by horizontal position
    /// </summary>
    public abstract void SortChildren();
}