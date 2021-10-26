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
        foreach (BaseNode node in dialogueGraph.nodes)
        {
            if (node.GetString() == "Start")
            {
                // Make this code the starting point
                dialogueGraph.current = node;
                break;
            }
        }

        parser = StartCoroutine(ParseNode());
    }

    private IEnumerator ParseNode()
    {
        BaseNode node = dialogueGraph.current;
        string data = node.GetString();
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
                dialogueGraph.current = port.Connection.node as BaseNode;
                break;
            }
        }

        parser = StartCoroutine(ParseNode());
    }
}
