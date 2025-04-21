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
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class SCR_NewUiManager : NetworkBehaviour
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
    private TextAsset exampleNPCDialogue;

    [SerializeField]
    private TextAsset scienceNPCDialogue;

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

    [SerializeField]
    private string currentFullLine;

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
    private QuestHandler questHandler;

    [SerializeField]
    private TextMeshProUGUI playerMoneyText;

    #region NetworkVariables
    [SerializeField]
    List<GameObject> AllGameobjects = new List<GameObject>();
    List<GameObject> DeactivatedObjects = new List<GameObject>();
    bool Ready = false;

    NetworkVariable<bool> PreReadyStatus = new NetworkVariable<bool>(false);

    private bool clientReady = false;

    public GameObject playerPrefab;
    #endregion


    private void Start()
    {
        //disable the cameras for all the players so they don't overlap with the 2D scene camera
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponentInChildren<Camera>().enabled = false;
            Debug.Log("aaaaa");
        }

        // existing initialization...
        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();
        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();

        // Subscribe to network variable changes
        playerDataHandler.playerMoney.OnValueChanged += OnPlayerMoneyChanged;

        // Set the initial text value
        OnPlayerMoneyChanged(playerDataHandler.playerMoney.Value, playerDataHandler.playerMoney.Value);

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

        exampleNPCDialogue = Resources.Load<TextAsset>("InkJsons/ExampleNPC");
        scienceNPCDialogue = Resources.Load<TextAsset>("InkJsons/PokingTheWhale_start");

        dialoguePanel = GameObject.Find("DialogueBox");
        dialoguePanel.SetActive(false);
        dialogueIsPlaying = false;
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;

        foreach (GameObject choice in choices)
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

        //something from here is not loading correctly in build

        questTrackerTitle = GameObject.Find("QuestTrackerTitle").GetComponent<TextMeshProUGUI>();
        questTrackerInfo = GameObject.Find("QuestTrackerInfo").GetComponent<TextMeshProUGUI>();
        questTrackerBackground = GameObject.Find("QuestTrackerBackground");

        questTrackerInfo.enabled = false;
        questTrackerBackground.SetActive(false);

        npcLocation = GameObject.Find("NPCLocation");
        npcLocation.SetActive(false);

        spoonsNPCLocation = GameObject.Find("SpoonsNPCTrackerLoc");

        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();

        questHandler = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        //player/ship stat variables
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);
        repairCostText.text = repairCost.ToString();

        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();
        playerMoneyText.text = playerDataHandler.playerMoney.Value.ToString();
        questInfoObject.questStamp.enabled = false;

        //Start of scene functions

        //no need to check for any complete conditions, because the complete condition of a cargo quest is getting to the end
        if(questHandler.hasCargoQuest.Value == true)
        {
            questHandler.hasCargoQuest.Value = false;
            playerDataHandler.playerMoney.Value += 100;
        }
    }

    private void Update()
    {
        if (!dialogueIsPlaying)
        {
            return;
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(typeTextCoroutine != null)
            {  
                StopCoroutine(typeTextCoroutine);   //stop the current text typing
                dialogueText.text = currentFullLine;    //set the text to the full line
                typeTextCoroutine = null;   //set the current coroutine to null to prevent any more typing
            }

            else if(!isShowingChoices)
            {
                ContinueStory();
            }
        }
    }

    private void OnPlayerMoneyChanged(float oldValue, float newValue)
    {
        playerMoneyText.text = newValue.ToString();
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
        else if (targetNPC == "Matthew")
        {
            EnterDialogueMode(scienceNPCDialogue);
        }
        else
        {

            EnterDialogueMode(spoonsNPCDialogue);
        }

    }

    public void Func_ExampleButtonPressed()
    {
        cityAnimator.SetBool("SpoonsPressed", true);
        spoonsButton.interactable = false;
        EnterDialogueMode(exampleNPCDialogue);
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
                questInfoObject.activeQuest = QuestInfo.questHeading.AppleADay; //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.Jenny;  //quest broker
                questInfoObject.researchImage.enabled = false; questInfoObject.deliveryImage.enabled = true;    //quest type image
                questInfoObject.jennyImage.enabled = true;  //npc image
                break;
            case QuestButton.QuestNames.Poking:
                questInfoText.text = "I'd like to obtain data concerning the Tulebreather's fog. Please meet me at The Weathered Spoony McSpoonface for more details.";
                questInfoObject.targetNPC = QuestInfo.npcBroker.Matthew;  //quest broker
                questInfoObject.activeQuest = QuestInfo.questHeading.Poking; //quest heading
                questInfoObject.researchImage.enabled = true; questInfoObject.deliveryImage.enabled = false;    //quest type image
                questInfoObject.jennyImage.enabled = false;  //npc image
                break;
            case QuestButton.QuestNames.Lightbulb:
                questInfoText.text = "Looking for data concerning the Voracious Angel Moth's photosensitivity. Please observe a Voracious Angel Moth in bright light and darkness for me, and bring me the results.";
                questInfoObject.activeQuest = QuestInfo.questHeading.LightbulbMoment;   //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;   //npc broker
                questInfoObject.researchImage.enabled = true; questInfoObject.deliveryImage.enabled = false;    //quest type image
                questInfoObject.jennyImage.enabled = false; //npc image
                break;
            case QuestButton.QuestNames.PostHaste:
                questInfoText.text = "Need these packages delivered ASAP by any willing Wayfarers. Please take them quickly!";
                questInfoObject.activeQuest = QuestInfo.questHeading.PostHaste;   //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;   //npc broker
                questInfoObject.researchImage.enabled = false; questInfoObject.deliveryImage.enabled = true;    //quest type image
                questInfoObject.jennyImage.enabled = false; //npc image
                break;
        }

    }
    public void Func_QuestBackButtonPressed()
    {
        cityAnimator.SetBool("QuestBoardPressed", false);
        cityAnimator.SetBool("QuestPressed", false);
        cityAnimator.SetBool("HasAcceptedQuest", false);

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
        questInfoObject.questStamp.enabled = false;
    }
    public void AcceptQuest()
    {
        Debug.Log("Accept Quest" + questInfoObject.activeQuest);
        questTrackerTitle.text = questInfoObject.activeQuest.ToString();
        questHandler.activeQuest.Value = questInfoObject.activeQuest.ToString();

        questTrackerInfo.text = questInfoText.text;
        targetNPC = questInfoObject.targetNPC.ToString();

        cityAnimator.SetBool("HasAcceptedQuest", true);

        //checks the type of quest being selected
        if(questInfoObject.questType == QuestInfo.questTypes.Cargo)
        {
            questHandler.hasCargoQuest.Value = true;
        }
        else
        {
            questHandler.hasCargoQuest.Value = false;
        }

        switch (targetNPC)
        {
            case "Jenny":
                npcLocation.SetActive(true);    //sets the mark above the area to visible
                npcLocation.transform.position = spoonsNPCLocation.transform.position;
                break;
            case "Matthew":
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
        playerDataHandler.playerMoney.Value -= repairCost;

        playerDataHandler.shipHealthGlobal.Value = 100.0f;
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);
        repairCostText.text = repairCost.ToString();
    }
    public void ShowStamp()
    {
        questInfoObject.questStamp.enabled = true;
    }
    public void Func_ReadyButtonPressed()
    {
        ReadyButtonPressed();
    }
    public void Func_TestButtonPress()
    {
        Debug.Log("BEEP");
    }


    #endregion Button Functions

    #region ReadyOperationsFunctions


    [ServerRpc(RequireOwnership = false)]
    public void loadGameServerRpc()
    {
        List<ulong> playerIDs = new List<ulong>();
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            playerIDs.Add(player.GetComponent<SCR_PlayerNetworkManager>().OwnerClientId);
        }
        Debug.Log("First foreach loop done");
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            Debug.Log("Destroyed");
            player.GetComponent<NetworkObject>().Despawn();

        }
        Debug.Log("Second foreach loop done");

        foreach (ulong playerID in playerIDs)
        {
            Debug.Log(playerID);
            GameObject playerInstance = Instantiate(playerPrefab);
            //playerInstance.GetComponent<NetworkObject>().SpawnAsPlayerObject(playerID);

            playerInstance.GetComponent<NetworkObject>().Spawn();
            playerInstance.GetComponent<NetworkObject>().ChangeOwnership(playerID);
            //playerInstance.transform.position = new Vector3(47, 31, 319);
            //playerInstance.GetComponent<SCR_PlayerNetworkManager>().bust();

        }
        Debug.Log("Third foreach loop done");

        NetworkManager.Singleton.SceneManager.LoadScene("SCN_DemoScene", LoadSceneMode.Single);
    }

    void ReadyButtonPressed()
    {
        GameObject Readytint = GameObjectCommon.FindChildwithTagStringLayer(this.gameObject, "ReadyTint", GameObjectCommon.NameTagLayer.Name);

        bool readystatus = Readytint.activeInHierarchy;
        Readytint.SetActive(!readystatus);

        if (!clientReady)
        {
            clientReady = true;
            if (!PreReadyStatus.Value)
            {
                ReadyedServerRpc(true);
            }

            else
            {
                Debug.Log("Loading Game Server");
                loadGameServerRpc();
                Debug.Log("Game Server Loaded");
            }
        }
        else
        {
            clientReady = false;
            ReadyedServerRpc(false);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void ReadyedServerRpc(bool newValue)
    {
        PreReadyStatus.Value = newValue;
    }

    #endregion

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
        yield return new WaitForSeconds(0.5f);

        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        dialogueTags.Clear();
        isShowingChoices = false;
        spoonsButton.interactable = true;
    }

    private void ContinueStory()
    {
        if(currentStory.canContinue)
        {
            currentFullLine = currentStory.Continue();  //caching the full text line to display if a click happens

            if(typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
            }

            typeTextCoroutine = StartCoroutine(TypeText(currentFullLine));

            if(dialogueTags.Contains("animate") && !isShowingChoices)   //show choices after the animations
            {
                DisplayChoices();
            }
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
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
        Debug.Log("Show Choices");
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
