using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class ChoiceSelectable : MonoBehaviour
    {
        public object element;
        
        [SerializeField] private Text choiceText;
        [SerializeField] private Button choiceButton;

        public Button ChoiceButton => choiceButton;

        public void Decide()
        {
            DialogueManager.SetDecision(element);
        }
        
        /// <summary>
        /// Set choice text to choice sentence
        /// </summary>
        /// <param name="choiceSentence">Choice's sentence</param>
        public void SetChoiceText(string choiceSentence)
        {
            choiceText.text = choiceSentence;
        }
    }
}