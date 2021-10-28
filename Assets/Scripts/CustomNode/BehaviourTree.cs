﻿using System;
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
        
        // Handle undo and redo when creating node
        Undo.RecordObject(this, "Behaviour Tree (CreatNode)");
        nodes.Add(node);
        
        AssetDatabase.AddObjectToAsset(node, this);
        Undo.RegisterCreatedObjectUndo(node, "Behaviour Tree (CreatNode)");
        
        AssetDatabase.SaveAssets();
        return node;
    }
    
    /// <summary>
    /// Delete node
    /// </summary>
    /// <param name="node"></param>
    public void DeleteNode(Node node)
    {
        Undo.RecordObject(this, "Behaviour Tree (DeleteNode)");
        nodes.Remove(node);
        
        // AssetDatabase.RemoveObjectFromAsset(node);
        // Handle undo and redo when deleting node
        Undo.DestroyObjectImmediate(node);
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
            // Handle undo and redo for decorator node
            Undo.RecordObject(decoratorNode, "Behaviour Tree (AddChild)");
            decoratorNode.child = child;
            EditorUtility.SetDirty(decoratorNode);
        }
        
        // Add composite node's children
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            // Handle undo and redo for composite node
            Undo.RecordObject(compositeNode, "Behaviour Tree (AddChild)");
            compositeNode.children.Add(child);
            EditorUtility.SetDirty(compositeNode);
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
            // Handle undo and redo for root node
            Undo.RecordObject(rootNode, "Behaviour Tree (RemoveChild)");
            rootNode.child = null;
            EditorUtility.SetDirty(rootNode);
        }
        
        // Remove decorator node's child
        DecoratorNode decoratorNode = parent as DecoratorNode;
        if (decoratorNode != null)
        {
            // Handle undo and redo for decorator node
            Undo.RecordObject(decoratorNode, "Behaviour Tree (RemoveChild)");
            decoratorNode.child = null;
            EditorUtility.SetDirty(decoratorNode);
        }
        
        // Remove composite node's children
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            // Handle undo and redo for composite node
            Undo.RecordObject(compositeNode, "Behaviour Tree (RemoveChild)");
            compositeNode.children.Remove(child);
            EditorUtility.SetDirty(compositeNode);
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
            children.Add(decoratorNode.child);
        }
        
        CompositeNode compositeNode = parent as CompositeNode;
        if (compositeNode != null)
        {
            return compositeNode.children;
        }

        return children;
    }
    
    public BehaviourTree Clone()
    {
        BehaviourTree tree = Instantiate(this);
        tree.rootNode = tree.rootNode.Clone();
        return tree;
    }
}