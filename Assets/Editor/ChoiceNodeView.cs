using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

public class ChoiceNodeView: INodeView
{
    private ChoiceNode choiceNode;
    
    public ChoiceNodeView(ChoiceNode node) : base("Assets/Editor/ChoiceNodeView.uxml")
    {
        this.node = node;
        choiceNode = node;
        title = node.name;
        viewDataKey = node.guid;
        
        // Set position
        style.left = node.position.x;
        style.top = node.position.y;
        
        CreateInputPorts();
        GenerateChoiceList();
        // CreateOutputPorts();
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
        
        Button addChoiceBtn = this.Q<Button>("add-choice-btn");
        addChoiceBtn.clicked += () => { AddChoicePort(); };
    }
    
    /// <summary>
    /// Generate choice list
    /// </summary>
    private void GenerateChoiceList()
    {
        // If there are no choices, return
        if (choiceNode.choices.Count < 0) return;
        
        // Generate choices
        foreach (Choice choice in choiceNode.choices)
        {
            // Generate port
            var generatedPort = GeneratePort(this, Direction.Output);
            generatedPort.portName = choice.choiceSentence;

            outputContainer.Add(generatedPort);
            RefreshPorts();
        }
    }
    
    /// <summary>
    /// Generate port in the node
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="portDirection">Port direction</param>
    /// <param name="capacity">The capacity of the port</param>
    /// <returns>The instantiated port in the node</returns>
    private Port GeneratePort(ChoiceNodeView node,
        Direction portDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    
    /// <summary>
    /// Add choice port
    /// </summary>
    /// <param name="overiddenPortName"></param>
    private void AddChoicePort(string overiddenPortName = "")
    {
        // Generate port
        var generatedPort = GeneratePort(this, Direction.Output);
        
        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        
        // Make choice port Name
        var choicePortName = string.IsNullOrEmpty(overiddenPortName)
            ? $"Choice {outputPortCount + 1}"
            : overiddenPortName;
        
        outputContainer.Add(generatedPort);
        RefreshPorts();
        
        choiceNode.choices.Add(new Choice());
        
        generatedPort.portName = choicePortName;

        // Add delete choice button
        // var deleteButton = new Button(() => RemovePort(choiceNode, generatedPort))
        // {
        //     text = "X"
        // };
        // generatedPort.contentContainer.Add(deleteButton);
        //
        // generatedPort.portName = choicePortName;
        //
        // // Add generated port to node
        // choiceNode.outputContainer.Add(generatedPort);
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