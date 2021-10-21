using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    
    public DialogueGraphView()
    {
        // Add style sheet for grid
        styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
        
        // Zoom in and out
        SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);
        
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        // Add grid
        var grid = new GridBackground();
        Insert(0, grid);
        grid.StretchToParentSize();
        
        AddElement(GenerateEntryPointNode());
    }

    /// <summary>
    /// Get compatible ports
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns>Compatible ports</returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        var compatiblePorts = new List<Port>();

        ports.ForEach(port =>
        {
            if (startPort != port && startPort.node != port.node)
                compatiblePorts.Add(port);
        });

        return compatiblePorts;
    }

    /// <summary>
    /// Generate port in the node
    /// </summary>
    /// <param name="node">Node</param>
    /// <param name="portDirection">Port direction</param>
    /// <param name="capacity">The capacity of the port</param>
    /// <returns>The instantiated port in the node</returns>
    private Port GeneratePort(DialogueNode node,
        Direction portDirection,
        Port.Capacity capacity = Port.Capacity.Single)
    {
        return node.InstantiatePort(Orientation.Horizontal, portDirection, capacity, typeof(float));
    }
    
    /// <summary>
    /// Generate dialogue entry point node in the graph view
    /// </summary>
    /// <returns>Entry Dialogue Node</returns>
    private DialogueNode GenerateEntryPointNode()
    {
        var node = new DialogueNode
        {
            title = "Start", // The name of the node
            GUID = Guid.NewGuid().ToString(),
            dialogueText = "Entry point",
            entryPoint = true
        };
        
        // Generate next port
        var generatedPort = GeneratePort(node, Direction.Output);
        generatedPort.portName = "Next";
        node.outputContainer.Add(generatedPort);
        
        // Remove expanded state in entry point node
        node.RefreshExpandedState();
        node.RefreshPorts();
        
        // Set the node position
        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }
    
    /// <summary>
    /// Add created dialogue node element 
    /// </summary>
    /// <param name="nodeName">Node name</param>
    public void CreateNode(string nodeName)
    {
        AddElement(CreateDialogueNode(nodeName));
    }
    
    /// <summary>
    /// Create dialogue node
    /// </summary>
    /// <param name="nodeName">Node name</param>
    /// <returns>New dialogue node with input port</returns>
    public DialogueNode CreateDialogueNode(string nodeName)
    {
        var dialogueNode = new DialogueNode
        {
            title = nodeName,
            dialogueText = nodeName,
            GUID = Guid.NewGuid().ToString()
        };
        
        // Set the dialogue node's title color
        dialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));

        // Generate input port
        var inputPort = GeneratePort(dialogueNode, Direction.Input, Port.Capacity.Multi);
        inputPort.portName = "Input";
        
        // Add input port to dialogue node
        dialogueNode.inputContainer.Add(inputPort);

        // Make choice button
        var button = new Button(() => { AddChoicePort(dialogueNode); })
        {
            text = "New choice"
        };

        // Add choice button to dialogue node as title
        dialogueNode.titleContainer.Add(button);
        
        // Make text field for dialogue node title
        var textField = new TextField(string.Empty);
        textField.RegisterValueChangedCallback(evt =>
        {
            dialogueNode.dialogueText = evt.newValue;
            dialogueNode.title = evt.newValue;
        });
        
        textField.SetValueWithoutNotify(dialogueNode.title);
        
        // Add node title text field to main container 
        dialogueNode.mainContainer.Add(textField);
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }

    /// <summary>
    /// Add choice port in dialogueNode
    /// </summary>
    /// <param name="dialogueNode">Dialogue Node</param>
    /// <param name="overiddenPortName">Port name. Default is ""</param>
    public void AddChoicePort(DialogueNode dialogueNode, string overiddenPortName = "")
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);

        // var oldLabel = generatedPort.contentContainer.Q<Label>("type");
        // generatedPort.contentContainer.Remove(oldLabel);
        
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        
        // Make choice port Name
        var choicePortName = string.IsNullOrEmpty(overiddenPortName)
            ? $"Choice {outputPortCount + 1}"
            : overiddenPortName;

        // Add text field in choice 
        var textField = new TextField
        {
            name = string.Empty,
            value = choicePortName
        };
        textField.RegisterValueChangedCallback(evt => generatedPort.portName = evt.newValue);
        generatedPort.contentContainer.Add(new Label(" "));
        generatedPort.contentContainer.Add(textField);
        
        // Add delete choice button
        var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
        {
            text = "X"
        };
        generatedPort.contentContainer.Add(deleteButton);
        
        generatedPort.portName = choicePortName;
        
        // Add generated port to node
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
    
    /// <summary>
    /// Remove choice port
    /// </summary>
    /// <param name="dialogueNode">Dialogue node</param>
    /// <param name="generatedPort"></param>
    private void RemovePort(DialogueNode dialogueNode, Port generatedPort)
    {
        var targetEdge = edges.ToList().Where(
            x => x.output.portName == generatedPort.portName && x.output.node == generatedPort.node).ToList();
        
        // If there are no target edge, return
        if (!targetEdge.Any()) return;
        var edge = targetEdge.First();
        edge.input.Disconnect(edge); // Disconnect the edge 
        RemoveElement(targetEdge.First()); // Remove the edge
        
        dialogueNode.outputContainer.Remove(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();



    }
}
