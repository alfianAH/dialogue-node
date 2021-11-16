using System.Collections;
using System.Collections.Generic;
using Ink.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Dialogue
{
    public class DialogueManager : SingletonBaseClass<DialogueManager>
    {
        [SerializeField] private TextAsset dialogueFile;
        [SerializeField] private Text speakerName;
        [SerializeField] private Text dialogueText;
        [SerializeField] private ChoiceSelectable choiceButtonPrefab;
        [SerializeField] private GameObject optionPanel;
        [SerializeField] private Image portraitPrefab;
        [SerializeField] private GameObject portraitHolder;

        private bool firstTime = true;
        private static Choice choiceSelected;
        private static Story story;
        private List<string> tags;

        private void Start()
        {
            tags = new List<string>();
            choiceSelected = null;
        }

        private void Update()
        {
            if (firstTime || Input.GetMouseButtonDown(0))
            {
                firstTime = false;
                Debug.Log(story.currentChoices.Count);
                if (story.canContinue)
                {
                    speakerName.text = "...";
                    AdvanceDialogue();
                    
                    // If there are choices, ...
                    if (story.currentChoices.Count != 0)
                    {
                        StartCoroutine(ShowChoices());
                    }
                }
                else
                {
                    FinishDialogue();
                }
            }
        }
        
        /// <summary>
        /// Set dialogue file
        /// </summary>
        /// <param name="dialogueOwner"></param>
        public void SetDialogue(DialogueOwner dialogueOwner)
        {
            dialogueFile = dialogueOwner.DialogueAsset;
            story = new Story(dialogueFile.text);
        }
        
        /// <summary>
        /// Action when dialogue is finished
        /// </summary>
        private void FinishDialogue()
        {
            Debug.Log("End of Dialogue");
            gameObject.SetActive(false);
        }

        private void AdvanceDialogue()
        {
            string currentSentence = story.Continue();
            
            ParseTags();
            
            // Type sentence letter by letter
            Debug.Log("Stop coroutine");
            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentSentence));
        }
        
        /// <summary>
        /// Type the sentence letter by letter
        /// </summary>
        /// <param name="sentence"></param>
        /// <returns></returns>
        private IEnumerator TypeSentence(string sentence)
        {
            dialogueText.text = "";

            foreach (char letter in sentence)
            {
                dialogueText.text += letter;
                yield return null;
            }
        }
        
        /// <summary>
        /// Create choices and show it to the screen
        /// </summary>
        /// <returns>Wait until one of the choice get selected</returns>
        private IEnumerator ShowChoices()
        {
            List<Choice> choices = story.currentChoices;

            foreach (Choice choice in choices)
            {
                ChoiceSelectable choiceButton = Instantiate(choiceButtonPrefab.gameObject, optionPanel.transform)
                    .GetComponent<ChoiceSelectable>();
                
                choiceButton.SetChoiceText(choice.text);
                choiceButton.element = choice;
                choiceButton.ChoiceButton.onClick.AddListener(() => { choiceButton.Decide(); });
            }

            optionPanel.SetActive(true);
            Debug.Log("Wait");
            yield return new WaitUntil(() => choiceSelected != null);
            
            AdvanceFromDecision();
        }
        
        /// <summary>
        /// Set decision from the choices
        /// </summary>
        /// <param name="element">The selected choice</param>
        public static void SetDecision(object element)
        {
            choiceSelected = (Choice) element;
            Debug.Log(choiceSelected);
            story.ChooseChoiceIndex(choiceSelected.index);
        }
        
        /// <summary>
        /// After a choice was made, turn off the panel and advance from that choice
        /// </summary>
        private void AdvanceFromDecision()
        {
            Debug.Log("Advance");
            optionPanel.SetActive(false); // Deactivate the option panel
            for (int i = 0; i < optionPanel.transform.childCount; i++)
            {
                Destroy(optionPanel.transform.GetChild(i).gameObject);
            }

            choiceSelected = null;
            AdvanceDialogue();
        }
        
        /// <summary>
        /// Parse tags in inky file
        /// Current tags: color
        /// </summary>
        private void ParseTags()
        {
            tags = story.currentTags;

            foreach (string t in tags)
            {
                string prefix = t.Split(' ')[0];
                string param = t.Split(' ')[1];

                switch (prefix.ToLower())
                {
                    case "color":
                        SetTextColor(param);
                        break;
                    case "sprite":
                        SetPortraits(param);
                        break;
                }
            }
        }
        
        /// <summary>
        /// Set dialogue text color
        /// </summary>
        /// <param name="color">Text color</param>
        private void SetTextColor(string color)
        {
            switch (color)
            {
                case "blue":
                    dialogueText.color = Color.blue;
                    break;
                case "black":
                    dialogueText.color = Color.black;
                    break;
                default:
                    Debug.LogError($"{color} is not available as a text color");
                    break;
            }
        }

        private void SetPortraits(string filenames)
        {
            foreach (string filename in filenames.Split(','))
            {
                Sprite portrait = Resources.Load<Sprite>(filename);
                Instantiate(portraitPrefab, portraitHolder.transform).GetComponent<Image>().sprite = portrait;
            }
        }
    }
}
