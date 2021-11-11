using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class BehaviourTreeView: GraphView
{
    public Action<INodeView> OnNodeSelected; 
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

        Undo.undoRedoPerformed += OnUndoRedo;
    }

    /// <summary>
    /// Handle undo and redo
    /// </summary>
    private void OnUndoRedo()
    {
        PopulateView(tree);
        AssetDatabase.SaveAssets();
    }
    
    /// <summary>
    /// Populate graph view
    /// </summary>
    /// <param name="tree">Behaviour Tree</param>
    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements.ToList());
        graphViewChanged += OnGraphViewChanged;

        if (tree.rootNode == null)
        {
            tree.rootNode = tree.CreateNode(typeof(RootNode)) as RootNode;
            EditorUtility.SetDirty(tree);
            AssetDatabase.SaveAssets();
        }
        
        // Creates node view
        tree.nodes.ForEach(n => CreateNodeView(n));
        
        // Create edges
        // Loop through nodes
        tree.nodes.ForEach(parentNode =>
        {
            // Get children of parent node
            var children = tree.GetChildren(parentNode);
            int i = 0;
            
            // Get parent node view
            INodeView parentView = FindNodeView(parentNode);
            
            // Loop through children nodes
            children.ForEach(childNode =>
            {
                // Get child node view
                INodeView childView = FindNodeView(childNode);
                
                // If child view is not null, ...
                if(childView != null)
                {
                    // Connect the child and the parent
                    if (parentView.output != null)
                    {
                        Edge edge = parentView.output.ConnectTo(childView.input);
                        AddElement(edge);
                    }
                    else
                    {
                        ChoiceNodeView choiceNodeView = parentView as ChoiceNodeView;
                        Edge edge = choiceNodeView?.GenerateChoiceList(i, childView);
                        if (edge != null)
                            AddElement(edge);
                        i++;
                    }
                }
                // Choice node doesn't have child yet
                else
                {
                    ChoiceNodeView choiceNodeView = parentView as ChoiceNodeView;
                    choiceNodeView?.GenerateChoiceList(i);
                    i++;
                }
            });
        });
    }
    
    /// <summary>
    /// Find node view by GUID
    /// </summary>
    /// <param name="node">Node from Editor</param>
    /// <returns>NodeView</returns>
    private INodeView FindNodeView(Node node)
    {
        if (node != null)
            return GetNodeByGuid(node.guid) as INodeView;
        
        return null;
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
            switch (elem)
            {
                // Remove nodes
                // If elem as node view is not null, ...
                case INodeView nodeView:
                    // Delete node
                    tree.DeleteNode(nodeView.node);
                    break;
                
                // Remove edges
                // If elem as edge is not null, ...
                case Edge edge:
                    // If edge's output node as node view (parent) and 
                    // edge's input node as node view (child) are not null
                    if(edge.output.node is INodeView parentView && 
                       edge.input.node is INodeView childView)
                    {
                        // Remove child
                        tree.RemoveChild(parentView.node, childView.node);
                    }

                    break;
            }
        });

        graphViewChange.edgesToCreate?.ForEach(edge =>
        {
            // If edge's output node as node view (parent) and 
            // edge's input node as node view (child) are not null
            if(edge.output.node is INodeView parentView && 
               edge.input.node is INodeView childView)
            {
                // Add child
                tree.AddChild(parentView.node, childView.node, edge.output.portName);
            }
        });
        
        // If there are moved elements, ...
        if (graphViewChange.movedElements != null)
        {
            // Sort all children nodes
            nodes.ForEach(n =>
            {
                INodeView nodeView = n as INodeView;

                nodeView?.SortChildren();
            });
        }

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
        
        // Choice nodes
        {
            var types = TypeCache.GetTypesDerivedFrom<ChoiceNode>();
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
        INodeView nodeView = new NodeView(node)
        {
            OnNodeSelected = OnNodeSelected
        };
        
        switch (node)
        {
            case ChoiceNode choiceNode:
                nodeView = new ChoiceNodeView(choiceNode)
                {
                    OnNodeSelected = OnNodeSelected
                };
                break;
        }
        
        AddElement(nodeView);
    }
    
    /// <summary>
    /// Update nodes' state
    /// </summary>
    public void UpdateNodeState()
    {
        nodes.ForEach(n =>
        {
            NodeView nodeView = n as NodeView;
            nodeView?.UpdateState();
        });
    }
}