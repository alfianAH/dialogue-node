using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ChoiceNodeView: INodeView
{
    public ChoiceNodeView(Node node) : base("Assets/Editor/ChoiceNodeView.uxml")
    {
        this.node = node;
        title = node.name;
        viewDataKey = node.guid;
        
        // Set position
        style.left = node.position.x;
        style.top = node.position.y;

        CreateInputPorts();
        // CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
    }

    protected override void SetupClasses()
    {
        switch (node)
        {
            case ChoiceNode _:
                AddToClassList("choice");
                break;
        }
    }

    protected override void CreateInputPorts()
    {
        switch (node)
        {
            case ChoiceNode _:
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
                break;
        }
        
        if (input != null)
        {
            input.portName = "";
            input.style.flexDirection = FlexDirection.Row;
            inputContainer.Add(input);
        }
    }
    
    public override void OnSelected()
    {
        base.OnSelected();
        OnNodeSelected?.Invoke(this);
    }

    public override void SortChildren() { }
}