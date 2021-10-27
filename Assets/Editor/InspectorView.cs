using UnityEditor;
using UnityEngine.UIElements;

public class InspectorView : VisualElement
{
    public new class UxmlFactory : UxmlFactory<InspectorView, UxmlTraits> { }

    private Editor editor;
    
    public InspectorView() { }
    
    /// <summary>
    /// Update selection 
    /// </summary>
    /// <param name="nodeView"></param>
    public void UpdateSelection(NodeView nodeView)
    {
        Clear(); // Clear previous selection
        
        // Destroy editor after creating one
        UnityEngine.Object.DestroyImmediate(editor);
        
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            editor.OnInspectorGUI();
        });
        
        Add(container);
    }
}
