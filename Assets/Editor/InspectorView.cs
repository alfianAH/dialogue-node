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
    public void UpdateSelection(INodeView nodeView)
    {
        Clear(); // Clear previous selection
        
        // Destroy editor after creating one
        UnityEngine.Object.DestroyImmediate(editor);
        
        editor = Editor.CreateEditor(nodeView.node);
        IMGUIContainer container = new IMGUIContainer(() =>
        {
            // If there is the target, ...
            if(editor.target)
            {
                editor.OnInspectorGUI(); // Show it in inspector
            }
        });
        
        Add(container);
    }
}
