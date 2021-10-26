using UnityEngine;
using XNode;

public class DialogueNode : BaseNode
{
	public enum Mood
	{
		Happy,
		Sad,
		Angry
	}
	[Input] public int entry;
	[Output] public int exit;
	[NodeEnum] public Mood mood;

	public string speakerName;
	[TextArea(3, 5)]
	public string dialogueLine;
	public Sprite sprite;

	[Output(dynamicPortList = true)] public string[] choices;

	public override string GetString()
	{
		return $"DialogueNode/{speakerName}/{dialogueLine}";
	}

	public override Sprite GetSprite()
	{
		return sprite;
	}
}