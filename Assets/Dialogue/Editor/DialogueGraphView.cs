using System;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    private readonly Vector2 defaultNodeSize = new Vector2(150, 200);
    
    public DialogueGraphView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
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
        
        dialogueNode.RefreshExpandedState();
        dialogueNode.RefreshPorts();
        dialogueNode.SetPosition(new Rect(Vector2.zero, defaultNodeSize));

        return dialogueNode;
    }
    
    /// <summary>
    /// Add choice port in dialogueNode
    /// </summary>
    /// <param name="dialogueNode">Dialogue Node</param>
    private void AddChoicePort(DialogueNode dialogueNode)
    {
        var generatedPort = GeneratePort(dialogueNode, Direction.Output);
        
        var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count;
        generatedPort.portName = $"Choice {outputPortCount}";
        
        // Add generated port to node
        dialogueNode.outputContainer.Add(generatedPort);
        dialogueNode.RefreshPorts();
        dialogueNode.RefreshExpandedState();
    }
}
