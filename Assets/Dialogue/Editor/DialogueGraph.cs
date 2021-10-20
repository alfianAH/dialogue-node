using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
    private DialogueGraphView graphView;
    private string fileName = "New Narrative";
    
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
        var fileNameTextField = new TextField("File Name:");
        fileNameTextField.SetValueWithoutNotify(fileName);
        fileNameTextField.MarkDirtyRepaint();
        fileNameTextField.RegisterValueChangedCallback(evt => fileName = evt.newValue);
        
        toolbar.Add(fileNameTextField);
        
        // Save data button
        toolbar.Add(new Button( () => RequestDataOperation(true))
            {text = "Save Data"}
        );
        
        // Load data button
        toolbar.Add(new Button( () => RequestDataOperation(false))
            {text = "Load Data"}
        );
        
        // Add button to create new node
        var nodeCreateButton = new Button(() =>
        {
            graphView.CreateNode("Dialogue Node");
        }) {text = "Create Node"};

        toolbar.Add(nodeCreateButton);
        rootVisualElement.Add(toolbar);
    }

    private void OnDisable()
    {
        rootVisualElement.Remove(graphView);
    }
    
    /// <summary>
    /// Request save or load data
    /// </summary>
    /// <param name="save">Set to true to save, false to load</param>
    private void RequestDataOperation(bool save)
    {
        // Check file name is null or empty
        if (string.IsNullOrEmpty(fileName))
        {
            EditorUtility.DisplayDialog("Invalid file name!", "Please enter a valid name.", "OK");
        }

        var saveUtility = GraphSaveUtility.GetInstance(graphView);
        
        // If save is true, then save the graph
        if (save)
        {
            saveUtility.SaveGraph(fileName);
        }
        else // Else, load the graph
        {
            saveUtility.LoadGraph(fileName);
        }
    }
}
