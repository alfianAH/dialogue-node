using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class DialogueContainer: ScriptableObject
{
    public List<NodeLinkData> nodeLinks  = new List<NodeLinkData>();
    public List<DialogueNodeData> dialogueNodeDatas = new List<DialogueNodeData>();
    public List<ExposedProperty> exposedProperties = new List<ExposedProperty>();
}