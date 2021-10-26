using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree tree;

    private void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log1 = ScriptableObject.CreateInstance<DebugLogNode>();
        log1.message = "Hello1";

        var pause1 = ScriptableObject.CreateInstance<WaitNode>();
        
        var log2 = ScriptableObject.CreateInstance<DebugLogNode>();
        log2.message = "Hello 2";
        
        var pause2 = ScriptableObject.CreateInstance<WaitNode>();
        
        var log3 = ScriptableObject.CreateInstance<DebugLogNode>();
        log3.message = "Hello 3";
        
        var pause3 = ScriptableObject.CreateInstance<WaitNode>();
        
        var sequence = ScriptableObject.CreateInstance<SequencerNode>();
        sequence.children.Add(log1);
        sequence.children.Add(pause1);
        sequence.children.Add(log2);
        sequence.children.Add(pause2);
        sequence.children.Add(log3);
        sequence.children.Add(pause3);
        
        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = sequence;
        
        tree.rootNode = loop;
    }

    private void Update()
    {
        tree.Update();
    }
}
