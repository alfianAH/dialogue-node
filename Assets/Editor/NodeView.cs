﻿using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class NodeView: UnityEditor.Experimental.GraphView.Node
{
    public Action<NodeView> OnNodeSelected;
    public Node node;
    public Port input;
    public Port output;
    
    public NodeView(Node node)
    {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;
        
        // Set position
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        CreateOutputPorts();
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
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode _:
                break;
        }

        if (input != null)
        {
            input.portName = "";
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
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
                break;
            case RootNode _:
            case DecoratorNode _:
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
                break;
        }

        if (output != null)
        {
            output.portName = "";
            outputContainer.Add(output);
        }
    }
    
    public override void SetPosition(Rect newPos)
    {
        base.SetPosition(newPos);
        node.position.x = newPos.xMin;
        node.position.y = newPos.yMin;
    }

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
}