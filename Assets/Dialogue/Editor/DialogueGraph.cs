using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    
    /// <summary>
    /// Open dialogue graph window from menu "Graph/Dialogue Graph"
    /// </summary>
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }

    private void OnEnable()
    {
        ConstructGraphView();
        GenerateToolbar();
    }
    
    /// <summary>
    /// Construct new dialogue graph view
    /// </summary>
    private void ConstructGraphView()
    {
        // Set the name of dialogue graph view
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        
        // Stretch the graph view all over the editor window
        graphView.StretchToParentSize();
        
        rootVisualElement.Add(graphView);
    }
    
    /// <summary>
    /// Generate toolbar in dialogue graph
    /// </summary>
    private void GenerateToolbar()
    {
        var toolbar = new Toolbar();
        
        // Add button to create new node
        var nodeCreateButton = new Button(() =>
        {
            graphView.CreateNode("Dialogue Node");
        });
        
        nodeCreateButton.text = "Create Node";
        
        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}
