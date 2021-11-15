using UnityEngine;

public class DialogueNode: DecoratorNode
{
    public string charaName;
    public Characters character;
    [TextArea(3, 5)]
    public string dialogueText;
    public Sprite characterSprite;

    protected override void OnStart()
    {
        Debug.Log("Dialogue node: Start");
    }

    protected override void OnStop()
    {
        Debug.Log("Dialogue node: Stop");
    }

    protected override State OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("Next");
            return State.Success; // Return success
        }

        return State.Running;
    }
}

public enum Characters
{
    Alfian, Aldy, Hamdani
}