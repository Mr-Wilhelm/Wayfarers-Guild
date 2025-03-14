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
        Debug.Log("1");
        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();
        Debug.Log("2");

        // Subscribe to network variable changes
        playerDataHandler.playerMoney.OnValueChanged += OnPlayerMoneyChanged;
        Debug.Log("3");

        // Set the initial text value
        OnPlayerMoneyChanged(playerDataHandler.playerMoney.Value, playerDataHandler.playerMoney.Value);
        Debug.Log("4");

        //button variables
        spoonsButton = GameObject.Find("BUTTON_Spoons").GetComponent<Button>();
        Debug.Log("5");
        questButton = GameObject.Find("BUTTON_Quests").GetComponent<Button>();
        Debug.Log("6");
        QuestButton questButtonClass = questButton.gameObject.GetComponent<QuestButton>();
        Debug.Log("7");

        repairButton = GameObject.Find("BUTTON_RepairShip").GetComponent<Button>();
        Debug.Log("8");
        repairCostText = GameObject.Find("RepairCost").GetComponent<TextMeshProUGUI>();
        Debug.Log("9");
        repairCostText.text = repairCost.ToString();
        Debug.Log("10");

        //character sprites
        spoonsSprite = GameObject.Find("SPRITE_SpoonsLady");
        Debug.Log("11");

        //animator variables
        cityAnimator = Resources.Load<Animator>("CityAnimController");
        Debug.Log("12");
        cityAnimator = GetComponent<Animator>();
        Debug.Log("3");

        //dialogue variables
        spoonsNPCDialogue = Resources.Load<TextAsset>("InkJsons/NPC1");
        Debug.Log("14");
        spoonsQuestDialogue = Resources.Load<TextAsset>("InkJsons/SpoonsQuest");
        Debug.Log("15");
        dialoguePanel = GameObject.Find("DialogueBox");
        Debug.Log("16");
        dialoguePanel.SetActive(false);
        Debug.Log("17");
        dialogueIsPlaying = false;
        Debug.Log("18");
        dialogueText = dialoguePanel.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log("19");
        choicesText = new TextMeshProUGUI[choices.Length];
        Debug.Log("20");
        int index = 0;
        Debug.Log("21");

        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
            Debug.Log("bbbb");
        }

        //audio variables
        audioSource = GetComponent<AudioSource>();
        Debug.Log("22");

        //quest info variables
        questInfoObject = GameObject.Find("QuestInfo").GetComponent<QuestInfo>();
        Debug.Log("23");
        questInfoText = questInfoObject.GetComponentInChildren<TextMeshProUGUI>();
        Debug.Log("24");

        questInfoObject.gameObject.SetActive(false);
        Debug.Log("25");



        //something from here is not loading correctly in build

        questTrackerTitle = GameObject.Find("QuestTrackerTitle").GetComponent<TextMeshProUGUI>();
        Debug.Log("27");
        questTrackerInfo = GameObject.Find("QuestTrackerInfo").GetComponent<TextMeshProUGUI>();
        Debug.Log("28");
        questTrackerBackground = GameObject.Find("QuestTrackerBackground");
        Debug.Log("29");

        questTrackerInfo.enabled = false;
        Debug.Log("30");
        questTrackerBackground.SetActive(false);
        Debug.Log("31");

        npcLocation = GameObject.Find("NPCLocation");
        Debug.Log("32");
        npcLocation.SetActive(false);
        Debug.Log("33");

        spoonsNPCLocation = GameObject.Find("SpoonsNPCTrackerLoc");
        Debug.Log("34");

        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();
        Debug.Log("35");

        //player/ship stat variables
        repairCost = (100.0f - playerDataHandler.shipHealthGlobal.Value);
        Debug.Log("36");
        repairCostText.text = repairCost.ToString();
        Debug.Log("37");

        playerMoneyText = GameObject.Find("PlayerMoneyText").GetComponent<TextMeshProUGUI>();
        Debug.Log("38");
        playerMoneyText.text = playerDataHandler.playerMoney.Value.ToString();
        Debug.Log("39");
        questInfoObject.questStamp.enabled = false;
        Debug.Log("26");
    }

    private void DisableStamp()
    {
        
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
                questInfoObject.activeQuest = QuestInfo.questHeading.AppleADay; //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.Jenny;  //quest broker
                questInfoObject.researchImage.enabled = false; questInfoObject.deliveryImage.enabled = true;    //quest type image
                questInfoObject.jennyImage.enabled = true;  //npc image
                break;
            case QuestButton.QuestNames.Lightbulb:
                questInfoText.text = "Looking for data concerning the Voracious Angel Moth's photosensitivity. Please observe a Voracious Angel Moth in bright light and darkness for me, and bring me the results.";
                questInfoObject.activeQuest = QuestInfo.questHeading.LightbulbMoment;   //quest heading
                questInfoObject.targetNPC = QuestInfo.npcBroker.None;   //npc broker
                questInfoObject.researchImage.enabled = true; questInfoObject.deliveryImage.enabled = false;    //quest type image
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

        questTrackerInfo.text = questInfoText.text;
        targetNPC = questInfoObject.targetNPC.ToString();

        cityAnimator.SetBool("HasAcceptedQuest", true);


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
        else if(!currentStory.canContinue)
        {
            Debug.Log("No More Dialogue");
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
