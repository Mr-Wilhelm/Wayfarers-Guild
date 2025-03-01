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
using Unity.VisualScripting;
using UnityEditor.Animations;

public class SCR_NewUiManager : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField]
    private Button spoonsButton;

    [SerializeField]
    private Button questButton;

    [SerializeField]
    private Button repairButton;

    [SerializeField]
    private TextMeshProUGUI repairCostText;

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
    private TextAsset spoonsQuestDialogue;

    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Story currentStory;

    [SerializeField]
    private bool dialogueIsPlaying = false;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip textSound;

    [SerializeField]
    private List<string> dialogueTags = new List<string>();

    [Header("Choices UI")]
    [SerializeField]
    private GameObject[] choices;

    [SerializeField]
    private TextMeshProUGUI[] choicesText;

    private Coroutine typeTextCoroutine;

    [SerializeField]
    private bool isShowingChoices;

    [Header("QuestVariables")]

    [SerializeField]
    private QuestInfo questInfoObject;

    [SerializeField]
    private TextMeshProUGUI questInfoText;

    [SerializeField]
    private string appleADayText;

    [SerializeField]
    private TextMeshProUGUI questTrackerTitle;

    [SerializeField]
    private TextMeshProUGUI questTrackerInfo;

    [SerializeField]
    private GameObject questTrackerBackground;

    [SerializeField]
    private string targetNPC;

    private GameObject spoonsNPCLocation, thamesNPCLocation;

    [SerializeField]
    private GameObject npcLocation;

    [Header("Stats")]
    [SerializeField]
    private float repairCost;

    [Header("DDOL Objects")]
    [SerializeField]
    private SCR_PlayerDataHandler playerDataHandler;

    [SerializeField]
    private TextMeshProUGUI playerMoneyText;

    private void Start()
    {
        //disable the cameras for all the players so they don't overlap with the 2D scene camera
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponentInChildren<Camera>().enabled = false;
        }

        //button variables
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();
        questButton = GameObject.Find("BUTTON_Quests").GetComponent<Button>();
        QuestButton questButtonClass = questButton.gameObject.GetComponent<QuestButton>();

        repairButton = GameObject.Find("BUTTON_RepairShip").GetComponent<Button>();
        repairCostText = GameObject.Find("RepairCost").GetComponent<TextMeshProUGUI>();
        repairCostText.text = repairCost.ToString();
        
        //character sprites
        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");

        //animator variables
        cityAnimator = Resources.Load<Animator>("CityAnimController");
        cityAnimator = GetComponent<Animator>();

        //dialogue variables
        spoonsNPCDialogue = Resources.Load<TextAsset>("InkJsons/NPC1");
        spoonsQuestDialogue = Resources.Load<TextAsset>("InkJsons/SpoonsQuest");
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
        
        questTrackerTitle = GameObject.Find("QuestTrackerTitle").GetComponent<TextMeshProUGUI>();
        questTrackerInfo = GameObject.Find("QuestTrackerInfo").GetComponent<TextMeshProUGUI>();
        questTrackerBackground = GameObject.Find("QuestTrackerBackground");

        questTrackerInfo.enabled = false;
        questTrackerBackground.SetActive(false);

        npcLocation = GameObject.Find("NPCLocation");
        npcLocation.SetActive(false);

        spoonsNPCLocation = GameObject.Find("SpoonsNPCTrackerLoc");

        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();

        //player/ship stat variables
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);
        repairCostText.text = repairCost.ToString();

        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();
        playerMoneyText.text = playerDataHandler.playerMoney.Value.ToString();
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        else if (Input.GetKeyDown(KeyCode.Mouse0) && dialogueIsPlaying)  //check if dialogue is playing
        {
            dialogueTags = currentStory.currentTags;
            if (currentStory.canContinue)    //check if the text file has more dialogue (this bool is an ink plugin thing)
            {
                //if (cityAnimator.GetBool("HasChoices") == false)
                //{
                //    cityAnimator.SetBool("HasChoices", true);   //set the choices parameter in the animator
                //}
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

        if(targetNPC == "Jenny")
        {
            EnterDialogueMode(spoonsQuestDialogue);
        }
        else
        {

            EnterDialogueMode(spoonsNPCDialogue);
        }

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
    public void Func_QuestPressed(QuestButton quest)
    {
        questInfoObject.gameObject.SetActive(true);
        questInfoText.text = appleADayText;
        cityAnimator.SetBool("QuestPressed", true);

        switch (quest.questName)
        {
            case QuestButton.QuestNames.AppleADay:
                questInfoText.text = "Looking for willing Wayfarers to take Spoony's finest cider to Chicago Contrails. If you're interested, come to The Weathered Spoony McSpoonface for a chat.";
                questInfoObject.activeQuest = QuestInfo.questHeading.AppleADay;
                questInfoObject.targetNPC = QuestInfo.npcBroker.Jenny;
                questInfoObject.jennyImage.enabled = true;
                break;
            case QuestButton.QuestNames.Lightbulb:
                questInfoText.text = "Looking for data concerning the Voracious Angel Moth's photosensitivity. Please observe a Voracious Angel Moth in bright light and darkness for me, and bring me the results.";
                questInfoObject.activeQuest = QuestInfo.questHeading.LightbulbMoment;
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;
                questInfoObject.jennyImage.enabled = false;
                break;
        }

    }
    public void Func_QuestBackButtonPressed()
    {
        cityAnimator.SetBool("QuestBoardPressed", false);
        cityAnimator.SetBool("QuestPressed", false);
        spoonsButton.interactable = true;
        spoonsButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        Invoke("delayDespawnQuestInfo", 1.0f);
    }
    public void Func_QuestTrackerDropDown()
    {
        questTrackerInfo.enabled = true;
        questTrackerBackground.SetActive(true);
    }
    public void Func_QuestTrackerCollapse()
    {
        questTrackerInfo.enabled = false;
        questTrackerBackground.SetActive(false);
    }
    private void delayDespawnQuestInfo()
    {
        questInfoText.text = "";
        questInfoObject.gameObject.SetActive(false);
    }
    public void AcceptQuest()
    {
        Debug.Log("Accept Quest" + questInfoObject.activeQuest);
        questTrackerTitle.text = questInfoObject.activeQuest.ToString();

        questTrackerInfo.text = questInfoText.text;
        targetNPC = questInfoObject.targetNPC.ToString();

        switch (targetNPC)
        {
            case "Jenny":
                npcLocation.SetActive(true);
                npcLocation.transform.position = spoonsNPCLocation.transform.position;
                break;

            case "None":
                Debug.Log("No NPC to track");
                npcLocation.SetActive(false);   //disable again just in case it is active from a quest
                break;
        }

    }
    public void DenyQuest()
    {
        Debug.Log("Deny Quest" + questInfoObject.activeQuest);
    }

    public void Func_RepairButtonPress()
    {
        playerDataHandler.playerMoney.Value -= repairCost;    //subtract money
        playerMoneyText.text = playerDataHandler.playerMoney.Value.ToString();  //re-set the value of money

        playerDataHandler.shipHealthGlobal.Value = 100.0f;
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);   //reset repair cost to 0
        repairCostText.text = repairCost.ToString();
    }
    public void Func_TestButtonPress()
    {
        Debug.Log("BEEP");
    }


    #endregion Button Functions

    #region Ink Dialogue Stuff - Tutorial used found in link Below
    //https://youtu.be/vY0Sk93YUhA

    public void EnterDialogueMode(TextAsset inkJSON)    //starts dialogue with the text file as a parameter
    {
        currentStory = new Story(inkJSON.text); //gets a story object (this is an ink plugin thing)
        dialogueTags = currentStory.currentTags;
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
        dialogueTags.Clear();
        isShowingChoices = false;
    }

    private void ContinueStory()    //
    {
        if (currentStory.canContinue)
        {
            //Stops the current text typing from playing.
            //This fixes a bug where text overlaps from different dialogues
            if (typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
            }

            typeTextCoroutine = StartCoroutine(TypeText(currentStory.Continue())); //set text for the current line
            if(dialogueTags.Contains("animate") && !isShowingChoices)
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
        isShowingChoices = true;
        //make the other choices invisible
        for (int i = index; i < choices.Length; i++)  
        {
            choices[i].gameObject.SetActive(false);
        }

        if (dialogueTags.Contains("animate"))
            cityAnimator.SetBool("HasChoices", true);
            
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

            yield return new WaitForSeconds(7.5f * Time.deltaTime);    //typing speed (lower value is faster)
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("You made your choice");
        currentStory.ChooseChoiceIndex(choiceIndex);
        isShowingChoices = false;
        ContinueStory();
    }

    #endregion


}
