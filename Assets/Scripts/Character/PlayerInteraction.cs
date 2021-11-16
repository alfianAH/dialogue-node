using Dialogue;
using UnityEngine;
using UnityEngine.UI;

namespace Character
{
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private DialogueManager dialogueManager;
        [SerializeField] private Button interactButton;
        [SerializeField] private Text interactButtonText; 
        
        private void OnTriggerEnter(Collider other)
        {
            switch (other.tag)
            {
                case "NPC":
                    interactButtonText.text = "Talk";
                    HandleInteractButton(other);
                    break;
                case "Item":
                    interactButtonText.text = "Interact";
                    HandleInteractButton(other);
                    break;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            switch (other.tag)
            {
                case "NPC":
                    interactButton.gameObject.SetActive(false);
                    break;
                case "Item":
                    interactButton.gameObject.SetActive(false);
                    break;
            }
        }

        private void HandleInteractButton(Collider target)
        {
            DialogueOwner dialogueOwner = target.GetComponent<DialogueOwner>();
            interactButton.gameObject.SetActive(true);
            interactButton.onClick.RemoveAllListeners();
            interactButton.onClick.AddListener(() =>
            {
                dialogueManager.gameObject.SetActive(true);
                dialogueManager.SetDialogue(dialogueOwner);
                interactButton.gameObject.SetActive(false);
            });
        }
    }
}