using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class NodeView: INodeView
{
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
    protected override void SetupClasses()
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
    protected override void CreateInputPorts()
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

    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }
    
    /// <summary>
    /// Sort composite node children by horizontal position
    /// </summary>
    public override void SortChildren()
    {
        CompositeNode compositeNode = node as CompositeNode;
        if (compositeNode != null)
        {
            compositeNode.children.Sort(SortByVerticalPosition);
        }
    }
}