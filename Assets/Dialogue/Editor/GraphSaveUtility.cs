using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
{
    private DialogueGraphView targetGraphView;
    private DialogueContainer containerCache;
    
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
    
    /// <summary>
    /// Load graph
    /// </summary>
    /// <param name="fileName">Dialogue Graph file name</param>
    public void LoadGraph(string fileName)
    {
        containerCache = Resources.Load<DialogueContainer>(fileName);

        if (containerCache == null)
        {
            EditorUtility.DisplayDialog("File not Found", 
                $"Target dialogue graph file ({fileName}) does not exist!", "OK");
            return;
        }

        ClearGraph();
        CreateNodes();
        ConnectNodes();
    }
    
    /// <summary>
    /// Clear the dialogue graph
    /// </summary>
    private void ClearGraph()
    {
        // Set entry point (Start node) GUID 
        Nodes.Find(node => node.entryPoint).GUID = containerCache.nodeLinks[0].baseNodeGuid;

        foreach (var node in Nodes)
        {
            if (node.entryPoint) continue;
            
            // Remove edges that connected to this node
            Edges.Where(edge => edge.input.node == node).ToList()
                .ForEach(edge => targetGraphView.RemoveElement(edge));
            
            // Remove node
            targetGraphView.RemoveElement(node);
        }
    }

    /// <summary>
    /// Create nodes
    /// </summary>
    private void CreateNodes()
    {
        foreach (var nodeData in containerCache.dialogueNodeDatas)
        {
            // Add node
            var tempNode = targetGraphView.CreateDialogueNode(nodeData.dialogueText, Vector2.zero);
            tempNode.GUID = nodeData.guid;
            targetGraphView.AddElement(tempNode);

            // Add choice ports in node
            var nodePorts = containerCache.nodeLinks.Where(x => x.baseNodeGuid == nodeData.guid).ToList();
            nodePorts.ForEach(x => targetGraphView.AddChoicePort(tempNode, x.portName));
        }
    }
    
    /// <summary>
    /// Connect node with its target node
    /// </summary>
    private void ConnectNodes()
    {
        foreach (var node in Nodes)
        {
            // Get all node links 
            var connections = containerCache.nodeLinks.Where(
                x => x.baseNodeGuid == node.GUID).ToList();

            for (int j = 0; j < connections.Count; j++)
            {
                var targetNodeGuid = connections[j].targetNodeGuid;
                // Search target node
                var targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                
                // Link the node
                LinkNodes(node.outputContainer[j].Q<Port>(), (Port) targetNode.inputContainer[0]);
                
                targetNode.SetPosition(new Rect(
                    containerCache.dialogueNodeDatas.First(x => x.guid == targetNodeGuid).position,
                    targetGraphView.defaultNodeSize));
            }
        }
    }
    
    /// <summary>
    /// Link nodes with edge
    /// </summary>
    /// <param name="output">Output port</param>
    /// <param name="input">Input port</param>
    private void LinkNodes(Port output, Port input)
    {
        var tempEdge = new Edge
        {
            output = output,
            input = input
        };
        
        tempEdge.input.Connect(tempEdge);
        tempEdge.output.Connect(tempEdge);
        
        targetGraphView.Add(tempEdge);
    }
}