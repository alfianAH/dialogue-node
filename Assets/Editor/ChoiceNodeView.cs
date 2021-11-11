using System.Collections.Generic;
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
        // GenerateChoiceList();
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
    /// <param name="i"></param>
    /// <param name="childView"></param>
    public Edge GenerateChoiceList(int i, INodeView childView)
    {
        // If there are no choices, return
        if (choiceNode.choices.Count < 0) return null;

        Choice choice = choiceNode.choices[i];
        
        // Generate port
        var generatedPort = GeneratePort(Direction.Output);
        generatedPort.portName = choice.choiceSentence;
        Edge edge = generatedPort.ConnectTo(childView.input);
        
        outputContainer.Add(generatedPort);
        RefreshPorts();
        return edge;
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
        return InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
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
            ? $"Choice {outputPortCount + 1}"
            : overiddenPortName;
        
        choiceNode.choices.Add(new Choice());
        
        generatedPort.portName = choicePortName;

        // Add delete choice button
        // var deleteButton = new Button(() => RemovePort(generatedPort))
        // {
        //     text = "X"
        // };
        // generatedPort.contentContainer.Add(deleteButton);
        //
        // // Add generated port to node
        outputContainer.Add(generatedPort);
        Outputs.Add(generatedPort);
        
        RefreshPorts();
    }

    // private void RemovePort(Port generatedPort)
    // {
    //     outputContainer.Remove(generatedPort);
    // }

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