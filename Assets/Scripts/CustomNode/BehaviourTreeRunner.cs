using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree tree;

    private void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "Hellow";
        
        var loop = ScriptableObject.CreateInstance<RepeatNode>();
        loop.child = log;
        
        tree.rootNode = loop;
    }

    private void Update()
    {
        tree.Update();
    }
}
