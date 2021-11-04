using System;
using System.Collections.Generic;

public abstract class ChoiceNode: Node
{
    public List<Choice> choices = new List<Choice>();
    
    public override Node Clone()
    {
        ChoiceNode node = Instantiate(this);
        node.choices.ForEach(choice =>
        {
            choice.child.Clone();
        });
        return node;
    }
}

[Serializable]
public class Choice
{
    public Node child;
    public string choiceName;
}