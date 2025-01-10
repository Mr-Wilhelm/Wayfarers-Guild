using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Ink.Runtime;  //ink stuff
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using System.Linq;

/// <summary>
/// To Anyone Other than myself (Will) trying to use this script
/// I apologise for the ENTIRE 2D section of the game being in this single script, and the horrors you are about to witness as a result
/// However with Networking stuff, i wasn't sure how practical it would be to have multiple scripts and networked game objects as a result
/// I thought that syncing stuff miiiiight be easier if its all in one script (this was written before attempting any networking, so i might be wrong)
/// Anyways, CTRL + F exists for a reason, and i've tried to make it as easy to read as possible, so have fun! ^_^
/// 
/// "You will witness true horror" - Malenia, Blade of Miquella
/// </summary>

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private SceneManagerScript sceneManager;
    private bool readyPressed = false;

    #region ship info
    [Header("Ship Info")]
    ShipInfo shipInfo;

    [SerializeField]
    private ShipStatManager shipStatManager;

    [SerializeField]
    private float maxShipHealth;
    #endregion

    #region UI Parent Screens
    [Header("Ui Screens")]

    [SerializeField]
    private GameObject cityUI;

    [SerializeField]
    private GameObject portThamesUI;

    [SerializeField]
    private GameObject upgradesUI;

    [SerializeField]
    private GameObject spoonsUI;
    #endregion

    #region Port Thames UI Elements
    [Header("Port Thames UI Elements")]

    [SerializeField]
    private GameObject cargoButton;

    [SerializeField]
    private GameObject upgradesButton;

    [SerializeField]
    private GameObject repairButton;

    [SerializeField]
    private float repairCost;

    [SerializeField]
    private GameObject portBackButton;
    #endregion

    #region Upgrade UI Elements
    [Header("Upgrades UI Elements")]

    [SerializeField]
    private GameObject equip1Button;

    [SerializeField]
    private GameObject equip2Button;

    [SerializeField]
    private GameObject equip3Button;

    [SerializeField]
    private GameObject owned1Button;

    [SerializeField]
    private GameObject owned2Button;

    [SerializeField]
    private GameObject owned3Button;

    [SerializeField]
    private GameObject upgrades1Button;

    [SerializeField]
    private GameObject upgrades2Button;

    [SerializeField]
    private GameObject upgrades3Button;
    #endregion

    #region Spoons UI Elements
    [Header("Spoons UI Elements")]
    [SerializeField]
    private GameObject spoonsBackButton;

    [SerializeField]
    private GameObject npc1Button;

    [SerializeField]
    private GameObject npc2Button;

    [SerializeField]
    private GameObject npc3Button;

    [SerializeField]
    private GameObject npc4Button;

    [SerializeField]
    private GameObject questBoardButton;

    [SerializeField]
    private GameObject questBoard;

    [SerializeField]
    private GameObject questBoardBackButton;

    [SerializeField]
    private GameObject questOne;

    [SerializeField]
    private GameObject questTwo;

    [SerializeField]
    private GameObject questThree;

    [SerializeField]
    private TextMeshProUGUI questOneText;

    [SerializeField]
    private TextMeshProUGUI questTwoText;

    [SerializeField]
    private TextMeshProUGUI questThreeText;

    [SerializeField]
    private int selectedQuest;

    [SerializeField]
    private TextMeshProUGUI currentQuestText;

    #endregion

    #region Constant UI Elements
    [Header("Constant UI Elements")]

    [SerializeField]
    private GameObject moneyCounter;

    [SerializeField]
    private float moneyAmount;

    [SerializeField]
    private GameObject readyButton;

    [SerializeField]
    private GameObject questButton;

    [SerializeField]
    private GameObject questButtonPivot;

    [SerializeField]
    private GameObject questCollapseArrow;

    private bool questDroppedDown;
    #endregion

    private void Awake()
    {

        //----------Constant UI Elements----------
        moneyCounter = GameObject.Find("Money Counter");
        moneyCounter.SetActive(true);

        readyButton = GameObject.Find("ReadyButton");
        readyButton.SetActive(true);

        questButton = GameObject.Find("QuestButton");
        questButton.SetActive(true);

        questButtonPivot = GameObject.Find("QuestButtonPivot");
        questButtonPivot.SetActive(true);

        questCollapseArrow = GameObject.Find("QuestCollapseArrow");
        questCollapseArrow.SetActive(false);
        //----------Constant UI Elements----------


        //----------Port Thames UI Elements----------
        cargoButton = GameObject.Find("CargoButton");
        upgradesButton = GameObject.Find("UpgradesButton");
        repairButton = GameObject.Find("RepairButton");
        portBackButton = GameObject.Find("ThamesBackButton");
        //----------Port Thames UI Elements----------

        //----------Upgrades UI----------
        equip1Button = GameObject.Find("Equip1Button");
        equip2Button = GameObject.Find("Equip2Button");
        equip3Button = GameObject.Find("Equip3Button");
        owned1Button = GameObject.Find("Owned1Button");
        owned2Button = GameObject.Find("Owned2Button");
        owned3Button = GameObject.Find("Owned3Button");
        upgrades1Button = GameObject.Find("Upgrades1Button");
        upgrades2Button = GameObject.Find("Upgrades2Button");
        upgrades3Button = GameObject.Find("Upgrades3Button");
        //----------Upgrades UI----------

        //----------Spoons UI----------
        //assigning the dialogue variables by accessing them in the folder
        spoonsBackButton = GameObject.Find("SpoonsBackButton");

        npc1Button = GameObject.Find("NPC1");
        npc2Button = GameObject.Find("NPC2");
        npc3Button = GameObject.Find("NPC3");
        npc4Button = GameObject.Find("NPC4");

        NPC1Dialogue = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Scripts/UIScripts/InkScripts/NPC1Test.json", typeof(TextAsset));
        NPC2Dialogue = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Scripts/UIScripts/InkScripts/NPC2.json", typeof(TextAsset));
        NPC3Dialogue = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Scripts/UIScripts/InkScripts/NPC3.json", typeof(TextAsset));
        NPC4Dialogue = (TextAsset)AssetDatabase.LoadAssetAtPath("Assets/Scripts/UIScripts/InkScripts/NPC4.json", typeof(TextAsset));

        dialoguePanel = GameObject.Find("Dialogue Box");
        dialoguePanel.SetActive(false);

        characterPortrait = dialogueText.transform.GetChild(0).GetComponent<Image>();

        questBoardButton = GameObject.Find("QuestBoard");        
        questBoard = GameObject.Find("QuestBoardBackground");

        questBoardBackButton = GameObject.Find("QuestBoardBackButton");

        questOne = GameObject.Find("Quest1Button");
        questTwo = GameObject.Find("Quest2Button");
        questThree = GameObject.Find("Quest3Button");

        questOneText = GameObject.Find("Quest1Text").GetComponent<TextMeshProUGUI>();
        questTwoText = GameObject.Find("Quest2Text").GetComponent<TextMeshProUGUI>();
        questThreeText = GameObject.Find("Quest3Text").GetComponent<TextMeshProUGUI>();

        currentQuestText = GameObject.Find("CurrentQuestInfo").GetComponent<TextMeshProUGUI>();
        currentQuestText.enabled = false;

        //TODO: Fix this Code to auto assign the text.

        //dialogueText = GameObject.Find("Dialogue Text").GetComponent<TextMeshProUGUI>();
        //Debug.Log("Assigned");
        //dialogueText.enabled = false;
        //Debug.Log("Hidden");

        //----------Spoons UI----------

        //----------City UI-----------

        //----------City UI-----------

        //----------UI Screens---------- call this last in the Awake function, otherwise no variables are assigned
        cityUI = GameObject.Find("CityElements");
        cityUI.SetActive(true);

        portThamesUI = GameObject.Find("PortThames");   //finds the parent object with all the UI elements childed
        portThamesUI.SetActive(false);

        upgradesUI = GameObject.Find("Upgrades");
        upgradesUI.SetActive(false);

        spoonsUI = GameObject.Find("Spoons");
        spoonsUI.SetActive(false);
        //----------UI Screens----------

        //----------Other Variables----------

        audioSource = GetComponent<AudioSource>();
        audioSource.enabled = false;

        //----------Other Variables----------
        repairCost = 10.0f;
    }

    private void Start()
    {
        //gets the choice texts
        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach(GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }
    private void OnEnable()
    {
        shipStatManager = GameObject.Find("ShipStats").GetComponent<ShipStatManager>();
    }

    #region City UI Functions
    //----------City UI------------
    public void QuestButtonPress()
    {
        if(!questDroppedDown)
        {
            questButtonPivot.transform.localScale = new Vector3
                (questButtonPivot.transform.localScale.x,
                questButtonPivot.transform.localScale.y + 3.0f,
                questButtonPivot.transform.localScale.z);

            questDroppedDown = true;
            questCollapseArrow.SetActive(true);
            currentQuestText.enabled = true;
        }
    }

    public void CollapseQuestMenu()
    {
        if(questDroppedDown)
        {
            questButtonPivot.transform.localScale = new Vector3
                (questButtonPivot.transform.localScale.x,
                questButtonPivot.transform.localScale.y - 3.0f,
                questButtonPivot.transform.localScale.z);

            questDroppedDown = false;
            questCollapseArrow.SetActive(false);
            currentQuestText.enabled = false;
        }
    }

    public void BackButton()
    {
        Debug.Log("Back button pressed");
    }



    public void FawkesRepairsButton()
    {
        Debug.Log("Fawkes repair button pressed");
    }

    public void ScieneceInstituteButton()
    {
        Debug.Log("Science Institute button pressed");
    }

    public void AirShipWrightButton()
    {
        Debug.Log("Air ship-wright button pressed");
    }

    public void BuckinghamPalaceButton()
    {
        Debug.Log("Buckingham palace button pressed");
    }

    public void ReadyButtonPress()
    {
        Debug.Log("Ready button pressed");

    }
    //----------City UI------------
#endregion

    #region Port Thames Functions
    //-----------Port Thames Functions------------
    public void PortThamesButton()
    {
        Debug.Log("Port Thames button pressed");
        portThamesUI.SetActive(true);
        cityUI.SetActive(false);
    }

    public void ThamesRepair()
    {
        //absolute value gets rid of negative values
        //moneyAmount -= shipStatManager.shipRepairCost;

        moneyAmount -= 100;
    }

    public void UpgradesButton()
    {
        upgradesUI.SetActive(true);
        portThamesUI.SetActive(false);
    }

    public void ThamesBackButton()
    {
        portThamesUI.SetActive(false);
        cityUI.SetActive(true);
    }

    public void CargoButton()
    {
        Debug.Log("Cargo Button Pressed");
    }

    //-----------Port Thames Functions------------
    #endregion

    #region Upgrade UI Functions
    //----------Upgrades UI Functions----------
    public void ReturnFromUpgrades()
    {
        Debug.Log("Returning from port Thames");
        upgradesUI.SetActive(false);
        portThamesUI.SetActive(true);
    }
    public void EquipItem1()
    {
        Debug.Log("Item 1 Equipped");
        Debug.Log("Items 2 and 3 unequipped");
    }
    public void EquipItem2()
    {
        Debug.Log("Item 2 Equipped");
        Debug.Log("Items 1 and 3 unequipped");
    }
    public void EquipItem3()
    {
        Debug.Log("Item 3 Equipped");
        Debug.Log("Items 1 and 2 unequipped");
    }
    public void UpgradeItem1()
    {
        Debug.Log("Item 1 Upgraded");
    }
    public void UpgradeItem2()
    {
        Debug.Log("Item 2 Upgraded");
    }
    public void UpgradeItem3()
    {
        Debug.Log("Item 3 Upgraded");
    }
    //----------Upgrades UI Functions----------
    #endregion

    #region Spoons Functions
    //----------Spoons Functions----------
    public void SpoonsButton()
    {
        spoonsUI.SetActive(true);
        questBoard.SetActive(false);
        cityUI.SetActive(false);
    }

    public void ReturnFromSpoons()
    {
        spoonsUI.SetActive(false);
        cityUI.SetActive(true);
    }

    public void NPCDialogue1()
    {
        EnterDialogueMode(NPC1Dialogue);
        characterPortrait.enabled = true;
    }
    public void NPCDialogue2()
    {
        EnterDialogueMode(NPC2Dialogue);
        characterPortrait.enabled = false;
    }
    public void NPCDialogue3()
    {
        EnterDialogueMode(NPC3Dialogue);
        characterPortrait.enabled = false;
    }
    public void NPCDialogue4()
    {
        EnterDialogueMode(NPC4Dialogue);
        characterPortrait.enabled = false;
    }

    public void QuestBoard()
    {
        npc1Button.SetActive(false);
        npc2Button.SetActive(false);
        npc3Button.SetActive(false);
        npc4Button.SetActive(false);

        questBoard.SetActive(true);
        questBoardButton.SetActive(false);

        spoonsBackButton.SetActive(false);
        questBoardBackButton.SetActive(true);
    }

    public void CloseQuestBoard()
    {
        npc1Button.SetActive(true);
        npc2Button.SetActive(true);
        npc3Button.SetActive(true);
        npc4Button.SetActive(true);

        questBoard.SetActive(false);
        questBoardButton.SetActive(true);

        spoonsBackButton.SetActive(true);
        questBoardBackButton.SetActive(false);
    }

    public void SelectQuestOne()
    {
        selectedQuest = 1;
        currentQuestText.text = questOneText.text;

    }
    public void SelectQuestTwo()
    {
        selectedQuest = 2;
        currentQuestText.text = questTwoText.text;

    }
    public void SelectQuestThree()
    {
        selectedQuest = 3;
        currentQuestText.text = questThreeText.text;

    }
    //----------Spoons Functions----------
    #endregion

    private void Update()
    {
        if (!dialogueIsPlaying) //stops other dialogue from playing if there is dialogue currently playing
        {
            return;
        }
        
        if(Input.GetKeyDown(KeyCode.Mouse0))
        {
            Invoke("ContinueStory", 0.2f);  //continues the story after a brief delay, this gets the button clicked first
        }


        moneyCounter.GetComponent<TextMeshProUGUI>().text = "You have $" + moneyAmount.ToString();
    }

    #region Dialogue System Variables
    [Header("Dialogue System Variables")]
    [SerializeField]
    private TextAsset NPC1Dialogue;

    [SerializeField]
    private TextAsset NPC2Dialogue;

    [SerializeField]
    private TextAsset NPC3Dialogue;

    [SerializeField]
    private TextAsset NPC4Dialogue;

    [SerializeField]
    private GameObject dialoguePanel;

    [SerializeField]
    private TextMeshProUGUI dialogueText;

    [SerializeField]
    private Image characterPortrait;

    [SerializeField]
    private Story currentStory;

    [SerializeField]
    private bool dialogueIsPlaying = false;

    [SerializeField]
    private GameObject[] choices;

    [SerializeField]
    private TextMeshProUGUI[] choicesText;

    private Coroutine typeTextCoroutine;

    [SerializeField]
    private AudioSource audioSource;

    [SerializeField]
    private AudioClip textSound;
    #endregion

    #region Ink Dialogue Stuff - Tutorial used found in link Below
    //https://youtu.be/vY0Sk93YUhA
    public void EnterDialogueMode(TextAsset inkJSON)
    {
        currentStory = new Story(inkJSON.text); //sets the current story to the variable
        dialogueIsPlaying = true;
        dialoguePanel.SetActive(true);  //activates the dialogue panel
        audioSource.enabled = true;

        ContinueStory();    //Loads the next line of the story
    }

    private void ExitDialogueMode()
    {
        dialogueIsPlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
        audioSource.enabled = false;
    }

    private void ContinueStory()
    {
        List<string> tags = currentStory.currentTags;   //gets tags from the current story
        Debug.Log(tags.Count);

        if(currentStory.canContinue)
        {
            //Stops the current text typing from playing.
            //This fixes a bug where text overlaps from different dialogues
            if(typeTextCoroutine != null)
            {
                StopCoroutine(typeTextCoroutine);
            }

            typeTextCoroutine = StartCoroutine(TypeText(currentStory.Continue())); //set text for the current line
            //display dialogue choices
            DisplayChoices();   //shows button choices
        }
        else
        {
            ExitDialogueMode();
        }
    }

    /// <summary>
    /// Takes the string, in this case the text of the current story
    /// Convert it to an array of Chars
    /// Iterate throug the array, adding to it
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    private IEnumerator TypeText(string text)
    {
        dialogueText.text = "";
        foreach (char letter in text.ToCharArray())
        {
            dialogueText.text += letter;
            audioSource.clip = textSound;
            if(!audioSource.isPlaying)
            {
                audioSource.Play();
            }

            yield return new WaitForSeconds(10.0f * Time.deltaTime);
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;  //gets current choices from the ink file

        if(currentChoices.Count > choices.Length)
        {
            //unity is set up to support up to three choices so far, lemme know if you want more
            Debug.LogError("More choies were given than the Ui can support, Will made this, so ask him for help if necessary. Number of choices given:" + currentChoices.Count);
        }

        int index = 0;

        //enable the choice buttons for the amount of current choices from the ink story
        foreach (Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }



        for(int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false); //go through the remaining choices the UI supports and make sure they're hidden.
        }
    }

    public void MakeChoice(int choiceIndex)
    {
        //the parameter is passed on the button press in the unity editor,
        //in the section where you assign functions to buttons
        currentStory.ChooseChoiceIndex(choiceIndex);
    }

    #endregion
}
