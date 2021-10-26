using System;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
    /// <param name="tree"></param>
    internal void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        
        graphViewChanged -= OnGraphViewChanged;
        DeleteElements(graphElements);
        graphViewChanged += OnGraphViewChanged;
        
        tree.nodes.ForEach(n => CreateNodeView(n));
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
            NodeView nodeView = elem as NodeView;

            if (nodeView != null)
            {
                tree.DeleteNode(nodeView.node);
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