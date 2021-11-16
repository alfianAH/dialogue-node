using UnityEngine;

namespace Dialogue
{
    public class DialogueOwner : MonoBehaviour
    {
        [SerializeField] private TextAsset dialogueAsset;

        public TextAsset DialogueAsset
        {
            get => dialogueAsset;
            set => dialogueAsset = value;
        }
    }
}