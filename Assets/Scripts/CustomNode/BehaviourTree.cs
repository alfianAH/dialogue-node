using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu()]
public class BehaviourTree : ScriptableObject
{
    public Node rootNode;
    public Node.State treeState = Node.State.Running;
    public List<Node> nodes = new List<Node>();

    public Node.State Update()
    {
        // If root node's state is running, ...
        if(rootNode.state == Node.State.Running)
        {
            // Update the tree state
            treeState = rootNode.Update();
        }

        return treeState;
    }
    
    /// <summary>
    /// Create node 
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Node CreateNode(Type type)
    {
        // Make node with name and GUID
        Node node = CreateInstance(type) as Node;
        node.name = type.Name;
        node.guid = GUID.Generate().ToString();

        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();
        return node;
    }
    
    /// <summary>
    /// Delete node
    /// </summary>
    /// <param name="node"></param>
    public void DeleteNode(Node node)
    {
        nodes.Remove(node);
        AssetDatabase.RemoveObjectFromAsset(node);
        AssetDatabase.SaveAssets();
    }
}