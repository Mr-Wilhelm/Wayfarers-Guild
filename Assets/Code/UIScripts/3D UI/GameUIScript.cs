using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Unity.Collections;
using UnityEngine.InputSystem;
using Gravitas.Demo;
using UnityEngine.UI;
using Unity.VisualScripting;

public class GameUIScript : NetworkBehaviour
{
    public SCR_PlayerDataHandler playerDataHandler;

    public QuestHandler questHandlerObject;

    public NetworkVariable<FixedString128Bytes> questPrompt = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptHeading = new NetworkVariable<FixedString128Bytes>();
    public NetworkVariable<FixedString128Bytes> questPromptTask = new NetworkVariable<FixedString128Bytes>();

    [SerializeField]
    private TextMeshProUGUI questPromptText, questPromptHeadingText, questPromptTaskText;

    [SerializeField]
    private bool questPromptEnabled = false;

    public bool showCompendium = false;

    [SerializeField]
    private GravitasFirstPersonPlayerSubject activePlayer;

    //Animation Variables
    [SerializeField]
    private Animator gameAnimator;

    //UI Objects
    [SerializeField]
    private GameObject compendium;

    private bool hasShownCompendium;

    [SerializeField]
    private Button patriciaButton;

    [SerializeField]
    private Button hubertoButton;

    [SerializeField]
    private Button craneButton;

    [SerializeField]
    private TextMeshProUGUI patriciaInfoTitle, patriciaInfoStats, patriciaInfoDesc, patriciaLeftPageText;

    [SerializeField]
    private TextMeshProUGUI hubertoInfoTitle, hubertoInfoStats, hubertoInfoDesc, hubertoLeftPageText;

    [SerializeField]
    private TextMeshProUGUI craneInfoTitle, craneInfoStats, craneInfoDesc, craneLeftPageText;

    [SerializeField]
    private Image patriciaRightImage, hubertoRightImage, craneRightImage;

    //Tutorial UI Objects
    [SerializeField]
    private GameObject interactPrompt;

    [SerializeField]
    private GameObject controlsPrompt;

    [SerializeField]
    private GameObject wheelPrompt;

    [SerializeField]
    private GameObject fuelPrompt;

    [SerializeField]
    private GameObject ammoPrompt;

    [SerializeField]
    private GameObject fusePrompt;

    [SerializeField]
    private GameObject ballistaPrompts;

    [SerializeField]
    private GameObject ballistaZoomPrompt;

    [SerializeField]
    private GameObject stationControlsPrompt;

    [SerializeField]
    public bool playerIsInRange;

    [SerializeField]
    public bool isInEnginePrompt;

    public bool
        lookingAtWheel,
        lookingAtCompendium,
        lookingAtBallistaStorage,
        lookingAtFuelStorage,
        lookingAtDroppedBallista,
        lookingAtDroppedFuel,
        lookingAtHatch,
        lookingAtEngine,
        lookingAtFuseBox,
        lookingAtFuseShelf;

    [SerializeField]
    private bool isInteracting;

    //Start is called before the first frame update
    void Start()
    {
        playerDataHandler = GameObject.Find("PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>();

        questHandlerObject = GameObject.Find("PlayerQuestHandler").GetComponent<QuestHandler>();

        //getting the variables
        questPromptText = GameObject.Find("QuestPrompt").GetComponent<TextMeshProUGUI>();
        questPromptHeadingText = GameObject.Find("QuestPromptHeading").GetComponent<TextMeshProUGUI>();
        questPromptTaskText = GameObject.Find("QuestPromptTask").GetComponent<TextMeshProUGUI>();

        //getting the values from the Quest Handler for the active quest
        questPrompt.Value = "Hold Q to view current quest";
        questPromptHeading.Value = questHandlerObject.activeQuest.Value;
        questPromptTask.Value = questHandlerObject.activeQuestDescription.Value;

        //assinging the text to the value of the network string
        questPromptText.text = questPrompt.Value.ToString();
        questPromptHeadingText.text = questPromptHeading.Value.ToString();
        questPromptTaskText.text = questPromptTask.Value.ToString();

        questPromptText.gameObject.SetActive(true);
        questPromptHeadingText.gameObject.SetActive(false);
        questPromptTaskText.gameObject.SetActive(false);

        //Animation Assigning
        gameAnimator = Resources.Load<Animator>("GameUIController");
        gameAnimator = GetComponent<Animator>();

        //UI Object Assigning
        compendium = GameObject.Find("Compendium");

        patriciaButton = GameObject.Find("PatriciaButton").GetComponent<Button>();
        hubertoButton = GameObject.Find("HubertoButton").GetComponent<Button>();
        craneButton = GameObject.Find("CraneButton").GetComponent<Button>();

        patriciaLeftPageText = GameObject.Find("PatriciaLeftPageText").GetComponent<TextMeshProUGUI>();
        hubertoLeftPageText = GameObject.Find("HubertoLeftPageText").GetComponent<TextMeshProUGUI>();
        craneLeftPageText = GameObject.Find("CraneLeftPageText").GetComponent<TextMeshProUGUI>();

        patriciaInfoTitle = GameObject.Find("PatriciaHeading").GetComponent<TextMeshProUGUI>();
        patriciaInfoStats = GameObject.Find("PatriciaStats").GetComponent<TextMeshProUGUI>();
        patriciaInfoDesc = GameObject.Find("PatriciaText").GetComponent<TextMeshProUGUI>();

        hubertoInfoTitle = GameObject.Find("HubertoHeading").GetComponent<TextMeshProUGUI>();
        hubertoInfoStats = GameObject.Find("HubertoStats").GetComponent<TextMeshProUGUI>();
        hubertoInfoDesc = GameObject.Find("HubertoText").GetComponent<TextMeshProUGUI>();

        craneInfoTitle = GameObject.Find("CraneHeading").GetComponent<TextMeshProUGUI>();
        craneInfoStats = GameObject.Find("CraneStats").GetComponent<TextMeshProUGUI>();
        craneInfoDesc = GameObject.Find("CraneText").GetComponent<TextMeshProUGUI>();

        patriciaRightImage = GameObject.Find("PatriciaRight").GetComponent<Image>();
        hubertoRightImage = GameObject.Find("HubertoRight").GetComponent<Image>();
        craneRightImage = GameObject.Find("CraneRight").GetComponent<Image>();

        patriciaInfoTitle.enabled = false;
        patriciaInfoStats.enabled = false;
        patriciaInfoDesc.enabled = false;

        hubertoInfoTitle.enabled = false;
        hubertoInfoStats.enabled = false;
        hubertoInfoDesc.enabled = false;

        craneInfoTitle.enabled = false;
        craneInfoStats.enabled = false;
        craneInfoDesc.enabled = false;

        patriciaRightImage.enabled = false;
        hubertoRightImage.enabled = false;
        craneRightImage.enabled = false;

        compendium.SetActive(false);

        interactPrompt = GameObject.Find("InteractPrompt");
        //interactPrompt.SetActive(false);
        SetInteractPromptDebug(false, "190");

        controlsPrompt = GameObject.Find("Controls Prompts");

        wheelPrompt = GameObject.Find("Wheel Prompts");
        wheelPrompt.SetActive(false);

        fuelPrompt = GameObject.Find("FuelPickupText");
        fuelPrompt.SetActive(false);

        ammoPrompt = GameObject.Find("AmmoPickupText");
        ammoPrompt.SetActive(false);

        fusePrompt = GameObject.Find("FusePickupText");
        fusePrompt.SetActive(false);

        ballistaZoomPrompt = GameObject.Find("ScopePrompt");
        //ballistaZoomPrompt.SetActive(false);

        ballistaPrompts = GameObject.Find("BallistaPrompts");
        ballistaPrompts.SetActive(false);

        stationControlsPrompt = GameObject.Find("StationControlsPrompt");
        stationControlsPrompt.SetActive(false);

        if (IsOwner)
        {
            activePlayer = GameObject.Find("Player_0").GetComponent<GravitasFirstPersonPlayerSubject>();
        }

        //UI Prompt Function Calling
        StartCoroutine(ShowMovementControls());
    }

    private void Update()
    {
        //assigning which player is looking at stuff. This should go first in update
        if (activePlayer == null)
        {
            activePlayer = GameObject.Find("Player_1")?.GetComponent<GravitasFirstPersonPlayerSubject>();
        }

        //check to see if the player is interacting
        if (activePlayer.GetComponent<SCR_NewInteract>().interacting)
        {
            isInteracting = true;
        }
        else
        {
            isInteracting = false;
        }

        //quest prompt stuff
        if (Input.GetKeyDown(KeyCode.Q))
        {
            questPromptEnabled = !questPromptEnabled;
        }
        if (Input.GetKeyUp(KeyCode.Q))
        {
            questPromptEnabled = !questPromptEnabled;
        }

        if (questPromptEnabled)  //show elements of the quest prompt
        {
            questPromptText.gameObject.SetActive(false);
            questPromptHeadingText.gameObject.SetActive(true);
            questPromptTaskText.gameObject.SetActive(true);
        }
        else if (!questPromptEnabled)    //hide elements of the quest prompt
        {
            questPromptText.gameObject.SetActive(true);
            questPromptHeadingText.gameObject.SetActive(false);
            questPromptTaskText.gameObject.SetActive(false);
        }

        //SO APPARENTLY REMOVING CODE TO INTERACT MAKES INTERACTIONS WORK!????? IDK BUT THIS MAKES IT SO THE CLIENT CAN INTERACT FINE? THIS IS SOME BIZARRE SORCERY RIGHT HERE
        //compendium stuff
        //if (Input.GetKeyDown(KeyCode.F) && showCompendium)
        //{
        //    StartCoroutine(ShowCompendium());
        //}
        //else if (Input.GetKeyDown(KeyCode.F) && !showCompendium)
        //{
        //    StartCoroutine(HideCompendium());
        //}

        if (showCompendium)
            SetInteractPromptDebug(false, "277");
        //interactPrompt.SetActive(false);
        else if (!showCompendium)
            //SetInteractPromptDebug(true, "280");
            //interactPrompt.SetActive(true);

        //checks player range, if they're interacting, and if they're in the engine
        if (!isInEnginePrompt)
        {
            if (playerIsInRange && !isInteracting)
            {
                if (lookingAtWheel || lookingAtHatch || lookingAtFuelStorage || lookingAtEngine || lookingAtDroppedFuel || lookingAtDroppedBallista || lookingAtCompendium && !showCompendium || lookingAtBallistaStorage || lookingAtFuseBox || lookingAtFuseShelf)
                {
                    SetInteractPromptDebug(true, "290");
                    //interactPrompt.SetActive(true);
                }
                else
                {
                    //Debug.Log(lookingAtWheel + "Key 1");
                    SetInteractPromptDebug(false, "295");
                    //interactPrompt.SetActive(false);
                }
            }
            else if (!playerIsInRange || isInteracting)
            {
                SetInteractPromptDebug(false, "301");
                //interactPrompt.SetActive(false);
            }
        }
        else if (isInEnginePrompt && activePlayer.GetComponent<SCR_NewInteract>().objectBeingHeld == "Engine Food")
        {
            //Debug.Log("Show the prompt maybe?");
            if (playerIsInRange && lookingAtEngine)
            {
                SetInteractPromptDebug(true, "310");
                //interactPrompt.SetActive(true);
            }
            else if (!playerIsInRange || !lookingAtEngine)
            {
                SetInteractPromptDebug(false, "315");
                //interactPrompt.SetActive(false);
            }
        }
    }
    public IEnumerator ShowCompendium()    //shows the compendium
    {
        if (!hasShownCompendium)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            activePlayer.enabled = false;

            hasShownCompendium = true;
            gameAnimator.SetBool("showCompendium", true);
            yield return new WaitForSeconds(0.1f);
            compendium.SetActive(true);
        }
    }
    public IEnumerator HideCompendium()    //hides the compendium
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        activePlayer.enabled = true;

        hasShownCompendium = false;
        gameAnimator.SetBool("showCompendium", false);
        yield return new WaitForSeconds(0.75f);  //how long after the animation starts before the compendium can be interacted with again without bugs happening
        gameAnimator.SetBool("compendiumAnimDone", true);
        compendium.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        gameAnimator.SetBool("compendiumAnimDone", false);

    }

    //compendium buttons and appearance stuff
    public void ShowPatriciaInfo()
    {
        patriciaInfoTitle.enabled = true;
        patriciaInfoStats.enabled = true;
        patriciaInfoDesc.enabled = true;
        patriciaRightImage.enabled = true;

        hubertoInfoTitle.enabled = false;
        hubertoInfoStats.enabled = false;
        hubertoInfoDesc.enabled = false;
        hubertoRightImage.enabled = false;

        craneInfoTitle.enabled = false;
        craneInfoStats.enabled = false;
        craneInfoDesc.enabled = false;
        craneRightImage.enabled = false;


    }
    public void ShowHubertoInfo()
    {
        patriciaInfoTitle.enabled = false;
        patriciaInfoStats.enabled = false;
        patriciaInfoDesc.enabled = false;
        patriciaRightImage.enabled = false;

        hubertoInfoTitle.enabled = true;
        hubertoInfoStats.enabled = true;
        hubertoInfoDesc.enabled = true;
        hubertoRightImage.enabled = true;

        craneInfoTitle.enabled = false;
        craneInfoStats.enabled = false;
        craneInfoDesc.enabled = false;
        craneRightImage.enabled = false;
    }
    public void ShowCraneInfo()
    {
        patriciaInfoTitle.enabled = false;
        patriciaInfoStats.enabled = false;
        patriciaInfoDesc.enabled = false;
        patriciaRightImage.enabled = false;

        hubertoInfoTitle.enabled = false;
        hubertoInfoStats.enabled = false;
        hubertoInfoDesc.enabled = false;
        hubertoRightImage.enabled = false;

        craneInfoTitle.enabled = true;
        craneInfoStats.enabled = true;
        craneInfoDesc.enabled = true;
        craneRightImage.enabled = true;
    }
    public void PreviousPage()
    {
        patriciaInfoTitle.enabled = false;
        patriciaInfoStats.enabled = false;
        patriciaInfoDesc.enabled = false;
        patriciaRightImage.enabled = false;

        hubertoInfoTitle.enabled = false;
        hubertoInfoStats.enabled = false;
        hubertoInfoDesc.enabled = false;
        hubertoRightImage.enabled = false;

        craneInfoTitle.enabled = false;
        craneInfoStats.enabled = false;
        craneInfoDesc.enabled = false;
        craneRightImage.enabled = false;
    }

    //prompt functions
    private IEnumerator ShowMovementControls()
    {
        controlsPrompt.SetActive(true);
        yield return new WaitForSeconds(5);
        HideMovementControls();
    }
    private void HideMovementControls()
    {
        controlsPrompt.SetActive(false);
    }

    public IEnumerator ShowWheelControls()
    {
        wheelPrompt.SetActive(true);
        activePlayer.GetComponent<SCR_NewInteract>().hasUsedWheelBefore = true;
        yield return new WaitForSeconds(10);
        HideWheelControls();
        if (activePlayer.GetComponent<SCR_NewInteract>().interacting)
        {
            ShowControlsPrompt();
        }
    }
    public void HideWheelControls()
    {
        wheelPrompt.SetActive(false);
    }
    public void ToggleWheelControlsOn()
    {
        wheelPrompt.SetActive(true);
    }
    public void ToggleWheelControlsOff()
    {
        wheelPrompt.SetActive(false);
    }

    public IEnumerator ShowFuelPrompt()
    {
        fuelPrompt.SetActive(true);
        activePlayer.GetComponent<SCR_NewInteract>().hasHadFuelBefore = true;
        yield return new WaitForSeconds(5);
        fuelPrompt.SetActive(false);
    }
    public void HideFuelPrompt()
    {
        fuelPrompt.SetActive(false);
    }
    public IEnumerator ShowAmmoPrompt()
    {
        ammoPrompt.SetActive(true);
        activePlayer.GetComponent<SCR_NewInteract>().hasHadAmmoBefore = true;
        yield return new WaitForSeconds(5);
        ammoPrompt.SetActive(false);
    }
    public void HideAmmoPrompt()
    {
        ammoPrompt.SetActive(false);
    }
    public IEnumerator ShowFusePrompt()
    {
        fusePrompt.SetActive(true);
        activePlayer.GetComponent<SCR_NewInteract>().hasHadFuseBefore = true;
        yield return new WaitForSeconds(5);
        fusePrompt.SetActive(false);
    }
    public void HideFusePrompt()
    {
        fusePrompt.SetActive(false);
    }
    public IEnumerator ShowBallistaControls()
    {
        ballistaPrompts.SetActive(true);
        if (ballistaZoomPrompt == null)
        {
            ballistaZoomPrompt = GameObject.Find("ScopePrompt");
        }
        if (playerDataHandler.ScopeUpgradeBought.Value == false)
        {
            ballistaZoomPrompt.SetActive(false);
        }
        else
        {
            ballistaZoomPrompt.SetActive(true);
        }
        activePlayer.GetComponent<SCR_NewInteract>().hasUsedBallistaBefore = true;
        yield return new WaitForSeconds(10);
        HideBallistaControls();
        if (activePlayer.GetComponent<SCR_NewInteract>().interacting)
        {
            ShowControlsPrompt();
        }

    }
    public void HideBallistaControls()
    {
        ballistaPrompts.SetActive(false);
    }
    public void ToggleBallistaControlsOn()
    {
        ballistaPrompts.SetActive(true);
    }
    public void ToggleBallistaControlsOff()
    {
        ballistaPrompts.SetActive(false);
    }
    public void ShowControlsPrompt()
    {
        if (activePlayer.GetComponent<SCR_NewInteract>().interacting)
            stationControlsPrompt.SetActive(true);
    }
    public void HideControlsPrompt()
    {
        stationControlsPrompt.SetActive(false);
    }

    public void SetInteractPromptDebug(bool Value) 
    {
        interactPrompt.SetActive(Value);
    }
    public void SetInteractPromptDebug(bool Value,string DebugMSG)
    {
        interactPrompt.SetActive(Value);
        //Debug.Log(Value +" "+ DebugMSG + " Key 1");
    }
}
