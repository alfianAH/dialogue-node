﻿public class DialogueWithChoiceNode: ChoiceNode
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return State.Success;
    }
}