using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class NodeParser : MonoBehaviour
{
    public DialogueGraph dialogueGraph;

    [SerializeField] private Text speakerName;
    [SerializeField] private Text dialogue;
    [SerializeField] private Image speakerImage;

    private Coroutine parser;
    
    private void Start()
    {
        foreach (var node in dialogueGraph.nodes)
        {
            BaseNode baseNode = (BaseNode) node;
            if (baseNode.GetString() == "Start")
            {
                // Make this code the starting point
                dialogueGraph.current = baseNode;
                break;
            }
        }

        parser = StartCoroutine(ParseNode());
    }
    
    /// <summary>
    /// Parse dialogue node
    /// </summary>
    /// <returns>Wait until left click</returns>
    private IEnumerator ParseNode()
    {
        BaseNode node = dialogueGraph.current;
        string data = node.GetString();

        if (data == "Start")
        {
            NextNode("exit");
            yield return null;
        }
        
        string[] dataParts = data.Split('/');
        
        if (dataParts[0] == "DialogueNode")
        {
            // Run dialogue
            speakerName.text = dataParts[1];
            dialogue.text = dataParts[2];
            speakerImage.sprite = node.GetSprite();
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0));
            NextNode("exit");
        }
    }
    
    /// <summary>
    /// Go to next node in graph 
    /// </summary>
    /// <param name="fieldName"></param>
    private void NextNode(string fieldName)
    {
        // Find the port with this name
        if (parser != null)
        {
            StopCoroutine(parser);
            parser = null;
        }

        foreach (NodePort port in dialogueGraph.current.Ports)
        {
            if (port.fieldName == fieldName)
            {
                try
                {
                    dialogueGraph.current = port.Connection.node as BaseNode;
                    break;
                }
                catch (NullReferenceException)
                {
                    Debug.Log("Dialogue finish");
                }
            }
        }

        parser = StartCoroutine(ParseNode());
    }
}
