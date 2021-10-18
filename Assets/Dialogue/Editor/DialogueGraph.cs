using UnityEditor;
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
        // Set the name of dialogue graph view
        graphView = new DialogueGraphView
        {
            name = "Dialogue Graph"
        };
        
        // Stretch the graph view all over the editor window
        graphView.StretchToParentSize();
        
        rootVisualElement.Add(graphView);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
}
