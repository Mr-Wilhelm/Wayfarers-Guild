using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting.FullSerializer;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
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
    #endregion


    private bool readyPressed = false;

    private void Awake()
    {
        //----------Constant UI Elements----------
        moneyCounter = GameObject.Find("Money Counter");
        moneyCounter.SetActive(true);

        readyButton = GameObject.Find("ReadyButton");
        readyButton.SetActive(true);

        questButton = GameObject.Find("QuestButton");
        questButton.SetActive(true);
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

        //----------City UI-----------

        //----------City UI-----------

        //----------UI Screens---------- call this last in the Awake function, otherwise no variables are assigned
        cityUI = GameObject.Find("CityElements");
        cityUI.SetActive(true);

        portThamesUI = GameObject.Find("PortThames");   //finds the parent object with all the UI elements childed
        portThamesUI.SetActive(false);

        upgradesUI = GameObject.Find("Upgrades");
        upgradesUI.SetActive(false);
        //----------UI Screens----------

        repairCost = 10.0f;
    }

    #region City UI Elements
    //----------City UI------------
    public void QuestButtonPress()
    {
        Debug.Log("Quest button pressed");
    }

    public void BackButton()
    {
        Debug.Log("Back button pressed");
    }


    public void SpoonsButton()
    {
        Debug.Log("Spoons button pressed");
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
        if(readyPressed)
        {
            Debug.Log("Player unreadied");
            readyPressed = false;
        }
        else
        {
            Debug.Log("Player ready");
            readyPressed = true;
        }
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
        moneyAmount -= Mathf.Abs(maxShipHealth - shipStatManager.shipHealth);
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

    private void Update()
    {
        moneyCounter.GetComponent<TextMeshProUGUI>().text = "You have $" + moneyAmount.ToString();
    }
}
