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
    private Button portButton;

    [Header("Sprites")]
    [SerializeField]
    private GameObject spoonsSprite;

    [SerializeField]
    private GameObject portSprite;

    [SerializeField]
    private Image cloudBackground, cityBackground;

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
    private TextAsset spoonsQuestCompleteDialogue;

    [SerializeField]
    private TextAsset researchQuestCompleteDialogue;

    [SerializeField]
    private TextAsset portNPCDefaultDialogue;

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

    [SerializeField]
    private bool canContinueStory;

    [SerializeField]
    private bool showUpgrades;

    [SerializeField]
    private bool isInSpoons;

    [SerializeField]
    private bool isHoveringSpoons;

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

    private GameObject spoonsNPCLocation, thamesNPCLocation, spoonsNPCCompleteLocation;

    [SerializeField]
    private GameObject npcLocation;

    [SerializeField]
    private GameObject npcQuestCompleteLocation;

    [Header("Upgrades UI")]
    [SerializeField]
    private GameObject upgradesUI;

    [Header("Stats")]
    public float repairCost;

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
        portButton = GameObject.Find("BUTTON_Port").GetComponent<Button>();
        QuestButton questButtonClass = questButton.gameObject.GetComponent<QuestButton>();

        //character sprites
        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");
        portSprite = GameObject.Find("SPRITE_PortMan");

        cloudBackground = GameObject.Find("CityClouds").GetComponent<Image>();
        cityBackground = GameObject.Find("CityCity").GetComponent<Image>();

        //animator variables
        cityAnimator = Resources.Load<Animator>("CityAnimController");
        cityAnimator = GetComponent<Animator>();

        //dialogue variables
        spoonsNPCDialogue = Resources.Load<TextAsset>("InkJsons/NPC1");
        spoonsQuestDialogue = Resources.Load<TextAsset>("InkJsons/SpoonsQuest");

        spoonsQuestCompleteDialogue = Resources.Load<TextAsset>("InkJsons/SpoonsQuestComplete");
        researchQuestCompleteDialogue = Resources.Load<TextAsset>("InkJsons/ResearchQuestComplete");

        exampleNPCDialogue = Resources.Load<TextAsset>("InkJsons/ExampleNPC");
        scienceNPCDialogue = Resources.Load<TextAsset>("InkJsons/PokingTheWhale_start");

        portNPCDefaultDialogue = Resources.Load<TextAsset>("InkJsons/PortDefault");

        //Choices UI - its up here for whatever reason if its lower then Unity doesn't assign it
        upgradesUI = GameObject.Find("---UPGRADE UI---");
        upgradesUI.SetActive(false);

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

        questTrackerTitle = GameObject.Find("QuestTrackerTitle").GetComponent<TextMeshProUGUI>();
        questTrackerInfo = GameObject.Find("QuestTrackerInfo").GetComponent<TextMeshProUGUI>();
        questTrackerBackground = GameObject.Find("QuestTrackerBackground");

        questTrackerInfo.enabled = false;
        questTrackerBackground.SetActive(false);

        npcLocation = GameObject.Find("NPCLocation");
        npcLocation.SetActive(false);

        npcQuestCompleteLocation = GameObject.Find("NPCQuestCompleteLocation");
        npcQuestCompleteLocation.SetActive(false);

        spoonsNPCLocation = GameObject.Find("SpoonsNPCTrackerLoc");
        spoonsNPCCompleteLocation = GameObject.Find("SpoonsCompleteTrackerLocation");

        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();

        questHandler = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        //player/ship stat variables
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);

        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();
        playerMoneyText.text = playerDataHandler.playerMoney.Value.ToString();
        questInfoObject.questStamp.enabled = false;

        isInSpoons = false;
        isHoveringSpoons = false;

        //has cargo quest with no people attached
        if(questHandler.hasCargoQuest.Value == true && questHandler.hasJennyQuest.Value == false && questHandler.hasMatthewQuest.Value == false)
        {
            questHandler.hasCargoQuest.Value = false;
            playerDataHandler.playerMoney.Value += 100;
        }
        else if(questHandler.hasResearchQuest.Value == true && questHandler.hasJennyQuest.Value == false && questHandler.hasMatthewQuest.Value == false)
        {
            questHandler.hasResearchQuest.Value = false;
            playerDataHandler.playerMoney.Value += 200;
        }
        //has cargo quest from Jenny
        else if(questHandler.hasCargoQuest.Value == true && questHandler.hasJennyQuest.Value == true && questHandler.hasMatthewQuest.Value == false)
        {
            questHandler.hasCompletedJennyQuest.Value = true;
            npcQuestCompleteLocation.SetActive(true);
            npcQuestCompleteLocation.transform.position = spoonsNPCCompleteLocation.transform.position;
        }
        //has research quest from matthew
        else if(questHandler.hasResearchQuest.Value == true && questHandler.hasMatthewQuest.Value == true && questHandler.hasJennyQuest.Value == false)
        {
            questHandler.hasCompletedMatthewQuest.Value = true;
            npcQuestCompleteLocation.SetActive(true);
            npcQuestCompleteLocation.transform.position = spoonsNPCCompleteLocation.transform.position;
        }

    }
    private void Update()
    {
        if(isHoveringSpoons || isInSpoons)
        {
            cloudBackground.color = new Color(0.75f, 0.75f, 0.75f);
            cityBackground.color = new Color(0.75f, 0.75f, 0.75f);
        }
        else
        {
            cloudBackground.color = new Color(1.0f, 1.0f, 1.0f);
            cityBackground.color = new Color(1.0f, 1.0f, 1.0f);
        }

        if (!dialogueIsPlaying)
        {
            return;
        }
        if (!Input.GetMouseButtonDown(0))
        {
            return;
        }

        if(typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
            dialogueText.text = currentFullLine;
            typeTextCoroutine = null;

            if(currentStory.currentChoices.Count > 0)
            {
                DisplayChoices();
            }
            return;
        }

        if(isShowingChoices)
        {
            return;
        }

        if(currentStory.canContinue)
        {
            ContinueStory();
        }
        else
        {
            StartCoroutine(ExitDialogueMode());
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
        portButton.interactable = false;

        //twenty billion else if statements and im not sorry
        if (questHandler.hasCompletedJennyQuest.Value == true)
        {
            EnterDialogueMode(spoonsQuestCompleteDialogue);
            playerDataHandler.playerMoney.Value += 100;
            questHandler.hasCargoQuest.Value = false; questHandler.hasJennyQuest.Value = false; questHandler.hasCompletedJennyQuest.Value = false;
        }
        else if(questHandler.hasCompletedMatthewQuest.Value == true)
        {
            EnterDialogueMode(researchQuestCompleteDialogue);
            playerDataHandler.playerMoney.Value += 200;
            questHandler.hasResearchQuest.Value = false; questHandler.hasMatthewQuest.Value = false; questHandler.hasCompletedMatthewQuest.Value = false;
        }
        else if (targetNPC == "Jenny")
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
    public void Func_PortButtonPressed()
    {
        cityAnimator.SetBool("PortPressed", true);
        portButton.interactable = false;
        spoonsButton.interactable = false;
        EnterDialogueMode(portNPCDefaultDialogue);
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
        StartCoroutine(ExitDialogueMode());
   
    }
    public void Func_PortBackButtonPressed()
    {
        cityAnimator.SetBool("PortPressed", false);
        StartCoroutine(ExitDialogueMode());
    }

    public void Func_UpgradesBackButtonPressed()
    {
        spoonsButton.interactable = true;
        portButton.interactable = true;
        upgradesUI.SetActive(false);
    }
    public void Func_QuestButtonPressed()
    {
        Debug.Log("Quest Button Pressed");
        cityAnimator.SetBool("QuestBoardPressed", true);
        spoonsButton.interactable = false;
        portButton.interactable = false;

        spoonsButton.GetComponent<CanvasGroup>().blocksRaycasts = false;
        portButton.GetComponent<CanvasGroup>().blocksRaycasts = false;

    }
    public void Func_QuestPressed(QuestButton quest)
    {
        questInfoObject.gameObject.SetActive(true);
        questInfoObject.questStamp.enabled = false;
        questInfoText.text = appleADayText;
        cityAnimator.SetBool("QuestPressed", true);

        switch (quest.questName)
        {
            case QuestButton.QuestNames.AppleADay:
                questInfoText.text = "Looking for willing Wayfarers to take Spoony's finest cider to Chicago Contrails. If you're interested, come to The Weathered Spoony McSpoonface for a chat.";
                questInfoObject.activeQuest = QuestInfo.questHeading.AppleADay; //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.Jenny;  //quest broker
                questInfoObject.questType = QuestInfo.questTypes.Cargo;
                questInfoObject.researchImage.enabled = false; questInfoObject.deliveryImage.enabled = true;    //quest type image
                questInfoObject.jennyImage.enabled = true;  //npc image
                break;
            case QuestButton.QuestNames.Poking:
                questInfoText.text = "I'd like to obtain data concerning the Tulebreather's fog. Please meet me at The Weathered Spoony McSpoonface for more details.";
                questInfoObject.targetNPC = QuestInfo.npcBroker.Matthew;  //quest broker
                questInfoObject.activeQuest = QuestInfo.questHeading.Poking; //quest heading
                questInfoObject.questType = QuestInfo.questTypes.Research;
                questInfoObject.researchImage.enabled = true; questInfoObject.deliveryImage.enabled = false;    //quest type image
                questInfoObject.jennyImage.enabled = false;  //npc image
                break;
            case QuestButton.QuestNames.Lightbulb:
                questInfoText.text = "Looking for data concerning the Voracious Angel Moth's photosensitivity. Please observe a Voracious Angel Moth in bright light and darkness for me, and bring me the results.";
                questInfoObject.activeQuest = QuestInfo.questHeading.LightbulbMoment;   //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;   //npc broker
                questInfoObject.questType = QuestInfo.questTypes.Research;
                questInfoObject.researchImage.enabled = true; questInfoObject.deliveryImage.enabled = false;    //quest type image
                questInfoObject.jennyImage.enabled = false; //npc image
                break;
            case QuestButton.QuestNames.PostHaste:
                questInfoText.text = "Need these packages delivered ASAP by any willing Wayfarers. Please take them quickly!";
                questInfoObject.activeQuest = QuestInfo.questHeading.PostHaste;   //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;   //npc broker
                questInfoObject.questType = QuestInfo.questTypes.Cargo;
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
        portButton.interactable = true;

        spoonsButton.GetComponent<CanvasGroup>().blocksRaycasts = true;
        portButton.GetComponent<CanvasGroup>().blocksRaycasts = true;

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

        CheckQuestType();

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

    private void CheckQuestType()
    {

        //checks the type of quest being selected
        if (questInfoObject.questType == QuestInfo.questTypes.Cargo)
        {
            questHandler.hasCargoQuest.Value = true;
            questHandler.hasResearchQuest.Value = false;
        }
        else if(questInfoObject.questType == QuestInfo.questTypes.Research)
        {
            questHandler.hasCargoQuest.Value = false;
            questHandler.hasResearchQuest.Value = true;
        }
        //check who the quest belongs to
        if (questInfoObject.targetNPC == QuestInfo.npcBroker.Jenny)
        {
            questHandler.hasJennyQuest.Value = true;
            questHandler.hasMatthewQuest.Value = false;
        }
        else if (questInfoObject.targetNPC == QuestInfo.npcBroker.Matthew)
        {
            questHandler.hasJennyQuest.Value = false;
            questHandler.hasMatthewQuest.Value = true;
        }
        else
        {
            questHandler.hasJennyQuest.Value = false;
            questHandler.hasMatthewQuest.Value = false;
        }
    }

    public void DenyQuest()
    {
        Debug.Log("Deny Quest" + questInfoObject.activeQuest);
    }

    public void Func_RepairButtonPress()
    {
        RepairShip();
    }

    public void RepairShip()
    {
        if (playerDataHandler.playerMoney.Value >= repairCost)
        {
            playerDataHandler.playerMoney.Value -= repairCost;

            playerDataHandler.shipHealthGlobal.Value = playerDataHandler.shipMaxHealth.Value;
            repairCost = (playerDataHandler.shipMaxHealth.Value - playerDataHandler.shipHealthGlobal.Value);
        }
        else
        {
            Debug.Log("Not enough money to repair");
        }
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

    public void Func_AddSpoonsDarken()
    {
        isHoveringSpoons = true;
    }
    public void Func_RemoveSpoonsDarken()
    {
        isHoveringSpoons = false;
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

        if(inkJSON.name == "PortDefault")
        {
            currentStory.variablesState["money"] = playerDataHandler.playerMoney.Value;
            currentStory.variablesState["repairCost"] = repairCost;
        }
        else if(inkJSON.name == "NPC1" || inkJSON.name == "SpoonsQuest" || inkJSON.name == "ResearchQuest")
        {
            isInSpoons = true;
        }

        dialogueTags = currentStory.currentTags;
        dialogueIsPlaying = true;   
        dialoguePanel.SetActive(true);  //activate the dialogue panel

        ContinueStory();    //continue the story
    }

    private IEnumerator ExitDialogueMode()  //stops the dialogue
    {
        if(showUpgrades)
        {
            upgradesUI.SetActive(true);
            showUpgrades = false;
        }
        //yield return new WaitForSeconds(0.5f);

        dialogueIsPlaying = false;
        cityAnimator.SetBool("SpoonsPressed", false);
        cityAnimator.SetBool("PortPressed", false);
        cityAnimator.SetBool("HasChoices", false);

        yield return new WaitForSeconds(0.9f);
        isInSpoons = false;
        dialoguePanel.SetActive(false);
        audioSource.Stop();
        dialogueText.text = "";
        dialogueTags.Clear();
        isShowingChoices = false;

        yield return new WaitForSeconds(0.5f);
        spoonsButton.interactable = true;
        portButton.interactable = true;

    }

    private void ContinueStory()
    {
        if(!currentStory.canContinue)   //check if the dialogue is finished
        {
            Debug.Log("Story cannot continue");
            StartCoroutine(ExitDialogueMode());
            return;
        }

        dialogueTags = currentStory.currentTags;    //gets all the tags
        currentFullLine = currentStory.Continue();  //getting the full line of text

        if(typeTextCoroutine != null)
        {
            StopCoroutine(typeTextCoroutine);
        }

        if (dialogueTags.Contains("repair"))
        {
            RepairShip();
        }
        else if(dialogueTags.Contains("upgrade"))
        {
            showUpgrades = true;
        }

        typeTextCoroutine = StartCoroutine(TypeText(currentFullLine));  //type out the current full line

        if(currentStory.currentChoices.Count > 0)
        {
            DisplayChoices();   //display choices if the count is greater than 0 (if there are choices)
        }
        else if(dialogueTags.Contains("animate"))
        {
            DisplayChoices();   //also display choices on the animate tag (this and the choices will happen at the same time
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
        float typingCPS = 20.0f;    //characters per second
        float delay = 1.0f / typingCPS;
        foreach (char letter in text.ToCharArray()) //convert the text to a char array
        {
            dialogueText.text += letter;    //add each char to the string
            audioSource.clip = textSound;
            if (!audioSource.isPlaying)
            {
                audioSource.Play(); //play the audio
            }

            yield return new WaitForSeconds(delay);    //typing speed (lower value is faster)
        }
        typeTextCoroutine = null;
    }

    public void MakeChoice(int choiceIndex)
    {
        Debug.Log("You made your choice");
        cityAnimator = Resources.Load<Animator>("CityAnimController");
        cityAnimator = GetComponent<Animator>();
        cityAnimator.SetBool("HasChoices", false);
        currentStory.ChooseChoiceIndex(choiceIndex);
        isShowingChoices = false;
        npcLocation.SetActive(false);
        ContinueStory();
    }
    #endregion
}