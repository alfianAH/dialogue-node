using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraphView : GraphView
{
    public DialogueGraphView()
    {
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());
        
        AddElement(GenerateEntryPointNode());
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
        
        // Set the node position
        node.SetPosition(new Rect(100, 200, 100, 150));

        return node;
    }
}
