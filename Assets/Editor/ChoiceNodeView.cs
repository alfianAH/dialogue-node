using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
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
        SetupClasses();

        Label descriptionLabel = this.Q<Label>("description");
        descriptionLabel.bindingPath = "description";
        descriptionLabel.Bind(new SerializedObject(node));
        
        Button addChoiceBtn = this.Q<Button>("add-choice-btn");
        addChoiceBtn.clicked += () => { AddChoicePort(); };
    }
    
    /// <summary>
    /// Generate choice port with edge
    /// </summary>
    /// <param name="i"></param>
    /// <param name="childView"></param>
    public Edge GenerateChoiceList(int i, INodeView childView)
    {
        // If there are no choices, return
        if (choiceNode.choices.Count < 0) return null;

        Choice choice = choiceNode.choices[i];
        
        // Generate port
        var generatedPort = GeneratePort(Direction.Output);
        generatedPort.portName = choice.choiceName;
        AddDeleteButton(generatedPort);
        Edge edge = generatedPort.ConnectTo(childView.input);
        
        RefreshPorts();
        return edge;
    }
    
    /// <summary>
    /// Generate choice port without edge
    /// </summary>
    /// <param name="i"></param>
    public void GenerateChoiceList(int i)
    {
        // If there are no choices, return
        if (choiceNode.choices.Count < 0) return;
        
        Choice choice = choiceNode.choices[i];
        
        // Generate port
        var generatedPort = GeneratePort(Direction.Output);
        generatedPort.portName = choice.choiceName;
        AddDeleteButton(generatedPort);
        
        RefreshPorts();
    }
    
    /// <summary>
    /// Generate port in the node
    /// </summary>
    /// <param name="portDirection">Port direction</param>
    /// <param name="capacity">The capacity of the port</param>
    /// <returns>The instantiated port in the node</returns>
    private Port GeneratePort(Direction portDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        Port generatedPort = InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
        outputContainer.Add(generatedPort);
        return generatedPort;
    }
    
    /// <summary>
    /// Add choice port
    /// </summary>
    /// <param name="overiddenPortName"></param>
    private void AddChoicePort(string overiddenPortName = "")
    {
        // Generate port
        var generatedPort = GeneratePort(Direction.Output);
        
        var outputPortCount = outputContainer.Query("connector").ToList().Count;
        
        // Make choice port Name
        var choicePortName = string.IsNullOrEmpty(overiddenPortName)
            ? $"Choice {outputPortCount}"
            : overiddenPortName;
        choiceNode.choices.Add(new Choice{choiceName = choicePortName});
        generatedPort.portName = choicePortName;
        
        AddDeleteButton(generatedPort);
        // Outputs.Add(generatedPort);
        RefreshPorts();
    }
    
    /// <summary>
    /// Add delete button to delete generated port
    /// </summary>
    /// <param name="generatedPort"></param>
    private void AddDeleteButton(Port generatedPort)
    {
        // Add delete choice button
        var deleteButton = new Button(() => RemovePort(generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
    }
    
    /// <summary>
    /// Remove port from node
    /// </summary>
    /// <param name="generatedPort"></param>
    private void RemovePort(Port generatedPort)
    {
        var targetEdge = generatedPort.connections.ToList();
        // If there are any edges, ...
        if(targetEdge.Any())
        {
            // Disconnect edge
            Edge edge = targetEdge.First();
            generatedPort.Disconnect(edge);
        }
        
        outputContainer.Remove(generatedPort);
        RefreshPorts();
        
        foreach (var choice in choiceNode.choices.Where(
            choice => choice.choiceName == generatedPort.portName))
        {
            choiceNode.choices.Remove(choice);
            break;
        }
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