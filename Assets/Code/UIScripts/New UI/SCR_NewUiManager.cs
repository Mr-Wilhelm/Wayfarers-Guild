using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using Ink.Runtime;
using TMPro;
using NUnit.Framework.Constraints;
using System.Linq;

public class SCR_NewUiManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button spoonsButton;

    [SerializeField]
    private Button questButton;

    [Header("Sprites")]
    [SerializeField]
    private GameObject spoonsSprite;

    [Header("Animations")]
    [SerializeField]
    private Animator cityAnimator;

    [Header("Dialogue System Variables")]
    [SerializeField]
    private TextAsset spoonsNPCDialogue;

    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Story currentStory;

    [SerializeField]
    private bool dialogueIsPlaying = false;

    [Header("Choices UI")]
    [SerializeField]
    private GameObject[] choices;

    [SerializeField]
    private TextMeshProUGUI[] choicesText;

    private Coroutine typeTextCoroutine;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip textSound;

    [Header("QuestObjects")]

    [SerializeField]
    private QuestInfo questInfoObject;

    [SerializeField]
    private TextMeshProUGUI questInfoText;

    [SerializeField]
    private string appleADayText;

    private void Start()
    {
        //button variables
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();
        questButton = GameObject.Find("BUTTON_Quests").GetComponent<Button>();
        questButtonClass = questButton.gameObject.GetComponent<QuestButton>();

        //character sprites
        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");

        //animator variables
        cityAnimator = GetComponent<Animator>();

        //dialogue variables
        spoonsNPCDialogue = Resources.Load<TextAsset>("InkJsons/NPC1");
        dialoguePanel = GameObject.Find("DialogueBox");
        dialoguePanel.SetActive(false);
        dialogueIsPlaying = false;
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;

        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }

        //audio variables
        audioSource = GetComponent<AudioSource>();

        //quest info variables
        questInfoObject = GameObject.Find("QuestInfo").GetComponent<QuestInfo>();
        questInfoText = questInfoObject.GetComponentInChildren<TextMeshProUGUI>();
        questInfoObject.gameObject.SetActive(false);

        #region Quest_Info_Text


        #endregion
    }

    private void Update()
    {
        if(!dialogueIsPlaying)
        {
            return;
        }

        else if(Input.GetKeyDown(KeyCode.Mouse0) && dialogueIsPlaying)  //check if dialogue is playing
        {
            if(currentStory.canContinue)    //check if the text file has more dialogue (this bool is an ink plugin thing)
            {
                if (cityAnimator.GetBool("HasChoices") == false)    
                {
                    cityAnimator.SetBool("HasChoices", true);   //set the choices parameter in the animator
                }
                ContinueStory();    //continue the story (this is an ink plugin thing)
            }
            else
            {
                return;
            }
        }
    }

    #region Button Functions
    public void Func_SpoonsButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", true);
        spoonsButton.interactable = false;

        EnterDialogueMode(spoonsNPCDialogue);
    }

    public void Func_SpoonsBackButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", false);
        cityAnimator.SetBool("HasChoices", false);

        spoonsButton.interactable = true;
   
    }

    public void Func_QuestButtonPressed()
    {
        Debug.Log("Quest Button Pressed");
        cityAnimator.SetBool("QuestBoardPressed", true);
        spoonsButton.interactable = false;
        spoonsButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

    }
    public void Func_QuestPressed()
    {
        cityAnimator.SetBool("QuestPressed", true);
        questInfoObject.gameObject.SetActive(true);
        questInfoText.text = appleADayText;
    }
    public void Func_QuestBackButtonPressed()
    {
        cityAnimator.SetBool("QuestBoardPressed", false);
        spoonsButton.interactable = true;
        spoonsButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Invoke("delayDespawnQuestInfo", 1.0f);
    }
    public void Func_TestButtonPress()
    {
        Debug.Log("BEEP");
    }

    private void delayDespawnQuestInfo()
    {
        questInfoText.text = "";
        questInfoObject.gameObject.SetActive(false);
    }
    #endregion Button Functions

    #region Ink Dialogue Stuff - Tutorial used found in link Below
    //https://youtu.be/vY0Sk93YUhA

    public void EnterDialogueMode(TextAsset inkJSON)    //starts dialogue with the text file as a parameter
    {
        currentStory = new Story(inkJSON.text); //gets a story object (this is an ink plugin thing)
        dialogueIsPlaying = true;   
        dialoguePanel.SetActive(true);  //activate the dialogue panel

        ContinueStory();    //continue the story
    }

    private IEnumerator ExitDialogueMode()  //stops the dialogue
    {
        yield return new WaitForSeconds(0.0f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private void ContinueStory()    //
    {
        List<string> tags = currentStory.currentTags;   //gets tags from the current story
        Debug.Log(tags.Count);

        if (currentStory.canContinue)
        {
            //Stops the current text typing from playing.
            //This fixes a bug where text overlaps from different dialogues
            if (typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
            }

            typeTextCoroutine = StartCoroutine(TypeText(currentStory.Continue())); //set text for the current line
            DisplayChoices();   //shows button choices
        }
        else
        {
            ExitDialogueMode();
        }
    }

    private void DisplayChoices()
    {

        List<Choice> currentChoices = currentStory.currentChoices;  //gets a list of choices from the ink dialogue (Choice class is an ink plugin thing)

        if(currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices given than the UI can support - Will made this, ask him for help if you dont understand");
        }

        int index = 0;
        //enable and initialise the choices for the dialogue

        foreach(Choice choice in currentChoices)    //iterate through all choices, for each one, set choices active and display
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        //make the other choices invisible
        for (int i = index; i < choices.Length; i++)  
        {
            choices[i].gameObject.SetActive(false);
        }
            
    }

    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray()) //convert the text to a char array
        {
            dialogueText.text += letter;    //add each char to the string
            audioSource.clip = textSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); //play the audio
            }

            yield return new WaitForSeconds(20.0f * Time.deltaTime);    //typing speed
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("You made your choice");
        currentStory.ChooseChoiceIndex(choiceIndex);
        ContinueStory();
    }

    #endregion


}
