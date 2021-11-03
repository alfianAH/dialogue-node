using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
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

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
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
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
                break;
            case RootNode _:
                break;
        }

        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Row;
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
            output.style.flexDirection = FlexDirection.RowReverse;
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
            compositeNode.children.Sort(SortByVerticalPosition);
        }
    }
    
    /// <summary>
    /// Sort nodes by vertical position 
    /// </summary>
    /// <param name="above">Above node</param>
    /// <param name="below">Below node</param>
    /// <returns>Returns -1 if above node's y  position less than below one, else 1</returns>
    private int SortByVerticalPosition(Node above, Node below)
    {
        return above.position.y < below.position.y ? -1 : 1;
    }
    
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
}