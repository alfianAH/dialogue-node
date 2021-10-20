using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GraphSaveUtility
{
    private DialogueGraphView targetGraphView;
    private List<Edge> Edges => targetGraphView.edges.ToList();
    private List<DialogueNode> Nodes => targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();
    
    public static GraphSaveUtility GetInstance(DialogueGraphView targetGraphView)
    {
        return new GraphSaveUtility
        {
            targetGraphView = targetGraphView
        };
    }
    
    /// <summary>
    /// Save dialogue graph if there are any edges in the graph
    /// </summary>
    /// <param name="fileName">Dialogue graph file name</param>
    public void SaveGraph(string fileName)
    {
        // If there are no edges (connection) then return
        if (!Edges.Any()) return;

        // Create dialogue container
        var dialogueContainer = ScriptableObject.CreateInstance<DialogueContainer>();
        
        // Add connected ports only
        var connectedPorts = Edges.Where(x=> x.input.node != null).ToArray();

        // Looping through ports in graph 
        foreach (var port in connectedPorts)
        {
            var outputNode = (DialogueNode) port.output.node;
            var inputNode = (DialogueNode) port.input.node;
            
            // Add node links to dialogue container
            dialogueContainer.nodeLinks.Add(new NodeLinkData
            {
                baseNodeGuid = outputNode.GUID,
                portName = port.output.portName,
                targetNodeGuid = inputNode.GUID
            });
        }
        
        // Looping through nodes in graph 
        foreach (DialogueNode dialogueNode in Nodes.Where(node => !node.entryPoint))
        {
            dialogueContainer.dialogueNodeDatas.Add(new DialogueNodeData
            {
                guid = dialogueNode.GUID,
                dialogueText = dialogueNode.dialogueText,
                position = dialogueNode.GetPosition().position
            });
        }

        // Create resources folder if there aren't any in Assets folder
        if (!AssetDatabase.IsValidFolder("Assets/Resources"))
            AssetDatabase.CreateFolder("Assets", "Resources");
        
        // Create and save the asset
        AssetDatabase.CreateAsset(dialogueContainer, $"Assets/Resources/{fileName}.asset");
        AssetDatabase.SaveAssets();
    }

    public void LoadGraph(string fileName)
    {
        
    }
}