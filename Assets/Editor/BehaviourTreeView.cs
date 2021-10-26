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
    public void PopulateView(BehaviourTree tree)
    {
        this.tree = tree;
        DeleteElements(graphElements);
        
        tree.nodes.ForEach(n => CreateNodeView(n));
    }
    
    /// <summary>
    /// Create node in graph view
    /// </summary>
    /// <param name="node"></param>
    void CreateNodeView(Node node)
    {
        NodeView nodeView = new NodeView(node);
        AddElement(nodeView);
    }
}