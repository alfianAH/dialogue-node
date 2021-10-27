using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu]
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
    
    /// <summary>
    /// Add child node to parent node (decorator and composite node)
    /// Note: Action node doesn't have child
    /// </summary>
    /// <param name="parent">Parent Node</param>
    /// <param name="child">Child node</param>
    public void AddChild(Node parent, Node child)
    {
        // Add root node's child
        RootNode rootNode = parent as RootNode;
        if (rootNode != null)
        {
            rootNode.child = child;
        }
        
        // Add decorator node's child
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode != null)
        {
            decoratorNode.child = child;
        }
        
        // Add composite node's children
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            compositeNode.children.Add(child);
        }
    }
    
    /// <summary>
    /// Remove child node in parent node (decorator and composite node)
    /// Note: Action node doesn't have child
    /// </summary>
    /// <param name="parent">Parent child</param>
    /// <param name="child">Child node</param>
    public void RemoveChild(Node parent, Node child)
    {
        // Remove root node's child
        RootNode rootNode = parent as RootNode;
        if (rootNode != null)
        {
            rootNode.child = null;
        }
        
        // Remove decorator node's child
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode != null)
        {
            decoratorNode.child = null;
        }
        
        // Remove composite node's children
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            compositeNode.children.Remove(child);
        }
    }
    
    /// <summary>
    /// Get parent node's children
    /// </summary>
    /// <param name="parent">Parent node</param>
    /// <returns></returns>
    public List<Node> GetChildren(Node parent)
    {
        List<Node> children = new List<Node>();
        
        // Remove root node's child
        RootNode rootNode = parent as RootNode;
        if (rootNode != null && rootNode.child != null)
        {
            children.Add(rootNode.child);
        }
        
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode != null && decoratorNode.child != null)
        {
            children.Add(decoratorNode);
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            return compositeNode.children;
        }

        return children;
    }
}