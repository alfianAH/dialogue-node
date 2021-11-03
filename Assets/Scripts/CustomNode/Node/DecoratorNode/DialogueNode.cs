using UnityEngine;

public class DialogueNode: DecoratorNode
{
    public string charaName;
    [TextArea(3, 5)]
    public string dialogueText;
    public Sprite characterSprite;
    
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}