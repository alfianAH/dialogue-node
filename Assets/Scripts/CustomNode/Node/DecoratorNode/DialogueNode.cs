using UnityEngine;

public class DialogueNode: DecoratorNode
{
    public string charaName;
    public Characters character;
    [TextArea(3, 5)]
    public string dialogueText;
    public Sprite characterSprite;
    
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            return State.Success; // Return success
        }

        return State.Running;
    }
}

public enum Characters
{
    Alfian, Aldy, Hamdani
}