using UnityEngine;
using UnityEngine.UI;

public class DialogueRunner : MonoBehaviour
{
    public BehaviourTree tree;
    
    [Header("User interface")] 
    [SerializeField] private Text dialogueText;
    [SerializeField] private Text speakerName;

    private void Start()
    {
        tree = tree.Clone();
        // tree.Bind();
    }

    private void Update()
    {
        tree.Update();
    }
}
