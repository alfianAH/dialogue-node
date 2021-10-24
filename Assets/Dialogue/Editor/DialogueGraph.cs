using UnityEditor;
using UnityEditor.Experimental.GraphView;
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
        GenerateMinimap();
        GenerateBlackboard();
    }
    
    /// <summary>
    /// Generate black board for properties
    /// </summary>
    private void GenerateBlackboard()
    {
        var blackboardView = new Blackboard(graphView);
        blackboardView.Add(new BlackboardSection
        {
            title = "Exposed Properties"
        });
        
        // Add property in blackboard
        blackboardView.addItemRequested = blackboard =>
        {
            graphView.AddPropertyToBlackBoard(new ExposedProperty());
        };
        
        blackboardView.SetPosition(new Rect(10, 30, 200, 300));
        
        graphView.Add(blackboardView);
        graphView.Blackboard = blackboardView;
        
    }
    
    /// <summary>
    /// Generate minimap in graph view
    /// </summary>
    private void GenerateMinimap()
    {
        var minimap = new MiniMap{anchored = true};
        var cords = graphView.contentViewContainer.WorldToLocal(new Vector2(maxSize.x - 10, 30));
        minimap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
        graphView.Add(minimap);
    }

    /// <summary>
    /// Construct new dialogue graph view
    /// </summary>
    private void ConstructGraphView()
    {
        // Set the name of dialogue graph view
        graphView = new DialogueGraphView(this)
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
