using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class BehaviourTreeView: GraphView
{
    public new class UxmlFactory : UxmlFactory<BehaviourTreeView, UxmlTraits>
    {
        
    }
    
    public BehaviourTreeView()
    {
        Insert(0, new GridBackground());
        
        var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/BehaviourTreeEditor.uss");
        styleSheets.Add(styleSheet);
    }
}