using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeView: UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    
    public NodeView(Node node): base("Assets/Editor/NodeView.uxml")
    {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;
        
        // Set position
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
        SetupClasses();
    }
    
    /// <summary>
    /// Set up class to each node to color it
    /// </summary>
    private void SetupClasses()
    {
        switch (node)
        {
            case ActionNode _:
                AddToClassList("action");
                break;
            case CompositeNode _:
                AddToClassList("composite");
                break;
            case DecoratorNode _:
                AddToClassList("decorator");
                break;
            case RootNode _:
                AddToClassList("root");
                break;
        }
    }
    
    /// <summary>
    /// Create input port in node
    /// </summary>
    private void CreateInputPorts()
    {
        switch (node)
        {
            case ActionNode _:
            case CompositeNode _:
            case DecoratorNode _:
                input = InstantiatePort(Orientation.Vertical, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode _:
                break;
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Column;
            inputContainer.Add(input);
        }
    }
    
    /// <summary>
    /// Create output port in node
    /// </summary>
    private void CreateOutputPorts()
    {
        switch (node)
        {
            case ActionNode _:
                break;
            case CompositeNode _:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case RootNode _:
            case DecoratorNode _:
                output = InstantiatePort(Orientation.Vertical, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }

        if (output != null)
        {
            output.portName = "";
            output.style.flexDirection = FlexDirection.ColumnReverse;
            outputContainer.Add(output);
        }
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

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
    
    /// <summary>
    /// Sort composite node children by horizontal position
    /// </summary>
    public void SortChildren()
    {
        CompositeNode compositeNode = node as CompositeNode;
        if (compositeNode != null)
        {
            compositeNode.children.Sort(SortByHorizontalPosition);
        }
    }
    
    /// <summary>
    /// Sort nodes by horizontal position 
    /// </summary>
    /// <param name="left">Left node</param>
    /// <param name="right">Right node</param>
    /// <returns>Returns -1 if left node's x  position less than right one, else 1</returns>
    private int SortByHorizontalPosition(Node left, Node right)
    {
        return left.position.x < right.position.x ? -1 : 1;
    }
}