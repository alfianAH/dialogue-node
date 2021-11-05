using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChoiceNode: Node
{
    public List<Choice> choices = new List<Choice>();
    
    public override Node Clone()
    {
        ChoiceNode node = Instantiate(this);
        node.choices.ForEach(choice =>
        {
            if(choice.child != null)
                choice.child.Clone();
        });
        return node;
    }
}

[Serializable]
public class Choice
{
    public Node child;
    [TextArea(1,2)]
    public string choiceSentence;
}