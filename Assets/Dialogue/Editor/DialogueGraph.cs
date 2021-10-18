using UnityEditor;
using UnityEngine;

public class DialogueGraph : EditorWindow
{
    /// <summary>
    /// Open dialogue graph window from menu "Graph/Dialogue Graph"
    /// </summary>
    [MenuItem("Graph/Dialogue Graph")]
    public static void OpenDialogueGraphWindow()
    {
        var window = GetWindow<DialogueGraph>();
        window.titleContent = new GUIContent("Dialogue Graph");
    }
}
