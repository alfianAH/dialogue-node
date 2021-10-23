using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
    private DialogueGraphView graphView;
    private EditorWindow editorWindow;
    private Texture2D indentationIcon;

    public void Init(EditorWindow editorWindow, DialogueGraphView graphView)
    {
        this.graphView = graphView;
        this.editorWindow = editorWindow;
        
        // Add indentatipn
        indentationIcon = new Texture2D(1, 1);
        indentationIcon.SetPixel(0, 0 , new Color(0, 0, 0, 0));
        indentationIcon.Apply();
    }
    
    /// <summary>
    /// Listing the elements that will be displayed in search list
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
    {
        var tree = new List<SearchTreeEntry>
        {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            
            // Add dialogue group in level 1
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
            // Add dialogue node in level 2
            new SearchTreeEntry(new GUIContent("Dialogue Node", indentationIcon))
            {
                userData = new DialogueNode(), 
                level = 2
            }
        };

        return tree;
    }

    public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
    {
        // Get world mouse position
        var worldMousePosition =
            editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent,
                context.screenMousePosition - editorWindow.position.position);
        // Convert world position to local position
        var localMousePosition = graphView.contentContainer.WorldToLocal(worldMousePosition);
        
        switch (searchTreeEntry.userData)
        {
            case DialogueNode dialogueNode:
                // Create dialogue node
                graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
            default:
                return false;
        }
    }
}