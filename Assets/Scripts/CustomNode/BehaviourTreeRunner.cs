using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourTreeRunner : MonoBehaviour
{
    private BehaviourTree tree;

    private void Start()
    {
        tree = ScriptableObject.CreateInstance<BehaviourTree>();

        var log = ScriptableObject.CreateInstance<DebugLogNode>();
        log.message = "Hellow";

        tree.rootNode = log;
    }

    private void Update()
    {
        tree.Update();
    }
}
