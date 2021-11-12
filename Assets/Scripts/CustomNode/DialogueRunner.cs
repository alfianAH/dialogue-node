using System;
using UnityEngine;
using UnityEngine.UI;

public class DialogueRunner : MonoBehaviour
{
    [Header("User interface")] 
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text speakerName;
    
    public BehaviourTree tree;

    private void Start()
    {
        tree = tree.Clone();
        tree.Bind();
    }

    private void Update()
    {
        tree.Update();
    }
}
