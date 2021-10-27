using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView: GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, GraphView.UxmlTraits> { }

    private BehaviourTree tree;
    
    public BehaviourTreeView()
    {
        // Add grid
        Insert(0, new GridBackground());
        
        this.AddManipulator(new ContentZoomer());
        this.AddManipulator(new ContentDragger());
        this.AddManipulator(new SelectionDragger());
        this.AddManipulator(new RectangleSelector());

        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
    
    /// <summary>
    /// Populate graph view
    /// </summary>
    /// <param name="tree">Behaviour Tree</param>
    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        
        // Creates node view
        tree.nodes.ForEach(n => CreateNodeView(n));
        
        // Create edges
        // Loop through nodes
        tree.nodes.ForEach(parentNode =>
        {
            // Get children of parent node
            var children = tree.GetChildren(parentNode);
            
            // Loop through children nodes
            children.ForEach(childNode =>
            {
                // Get parent node view
                NodeView parentView = FindNodeView(parentNode);
                // Get child node view
                NodeView childView = FindNodeView(childNode);
                
                Debug.Log($"Parent: {parentView.name} child: {childView}");
                
                // Connect the child and the parent
                Edge edge = parentView.output.ConnectTo(childView.input);

                AddElement(edge);
            });
        });
    }
    
    /// <summary>
    /// Find node view by GUID
    /// </summary>
    /// <param name="node">Node from Editor</param>
    /// <returns>NodeView</returns>
    private NodeView FindNodeView(Node node)
    {
        return GetNodeByGuid(node.guid) as NodeView;
    }
    
    /// <summary>
    /// Get compatible ports
    /// </summary>
    /// <param name="startPort"></param>
    /// <param name="nodeAdapter"></param>
    /// <returns>Return ports where endport's direction and node aren't startport's</returns>
    public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
    {
        return ports.ToList().Where(endPort =>
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
    }

    /// <summary>
    /// Handle graph view change
    /// </summary>
    /// <param name="graphViewChange"></param>
    /// <returns>graphViewChange</returns>
    private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
    {
        graphViewChange.elementsToRemove?.ForEach(elem =>
        {
            // Remove nodes
            // If elem as node view is not null, ...
            if (elem is NodeView nodeView)
            {
                // Delete node
                tree.DeleteNode(nodeView.node);
            }
            
            // Remove edges
            // If elem as edge is not null, ...
            if (elem is Edge edge)
            {
                // If edge's output node as node view (parent) and 
                // edge's input node as node view (child) are not null
                if(edge.output.node is NodeView parentView && 
                   edge.input.node is NodeView childView)
                {
                    // Remove child
                    tree.RemoveChild(parentView.node, childView.node);
                }
            }
        });

        graphViewChange.edgesToCreate?.ForEach(edge =>
        {
            // If edge's output node as node view (parent) and 
            // edge's input node as node view (child) are not null
            if(edge.output.node is NodeView parentView && 
               edge.input.node is NodeView childView)
            {
                // Add child 
                tree.AddChild(parentView.node, childView.node);
            }
        });

        return graphViewChange;
    }

    public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
    {
        // Action nodes
        {
            var types = TypeCache.GetTypesDerivedFrom<ActionNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", 
                    action => CreateNode(type));
            }
        }
        
        // Composite nodes
        {
            var types = TypeCache.GetTypesDerivedFrom<CompositeNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", 
                    action => CreateNode(type));
            }
        }
        
        // Decorator nodes
        {
            var types = TypeCache.GetTypesDerivedFrom<DecoratorNode>();
            foreach (var type in types)
            {
                evt.menu.AppendAction($"[{type.BaseType?.Name}] {type.Name}", 
                    action => CreateNode(type));
            }
        }
    }
    
    /// <summary>
    /// Create node from node type
    /// </summary>
    /// <param name="type">Node type</param>
    private void CreateNode(Type type)
    {
        Node node = tree.CreateNode(type);
        CreateNodeView(node);
    }

    /// <summary>
    /// Create node in graph view
    /// </summary>
    /// <param name="node"></param>
    private void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }
}