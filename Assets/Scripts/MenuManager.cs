using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Ui Screens")]

    [SerializeField]
    private GameObject cityUI;

    [SerializeField]
    private GameObject portThamesUI;

    [SerializeField]
    private GameObject upgradesUI;

    [Header("Port Thames UI Elements")]

    [SerializeField]
    private GameObject cargoButton;

    [SerializeField]
    private GameObject upgradesButton;

    [SerializeField]
    private GameObject repairButton;

    [SerializeField]
    private GameObject portBackButton;

    [Header("Constant UI Elements")]

    [SerializeField]
    private GameObject moneyCounter;

    [SerializeField]
    private GameObject readyButton;

    [SerializeField]
    private GameObject questButton;
    


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


        //----------UI Screens---------- call this last in the Awake function, otherwise no variables are assigned
        cityUI = GameObject.Find("CityElements");
        cityUI.SetActive(true);

        portThamesUI = GameObject.Find("PortThames");   //finds the parent object with all the UI elements childed
        portThamesUI.SetActive(false);

        upgradesUI = GameObject.Find("Upgrades");
        upgradesUI.SetActive(false);
        //----------UI Screens----------
    }

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

    //-----------Port Thames Functions------------

    public void PortThamesButton()
    {
        Debug.Log("Port Thames button pressed");
        portThamesUI.SetActive(true);
        cityUI.SetActive(false);
    }

    public void ThamesRepair()
    {
        Debug.Log("Repair Ship");
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

    //----------Upgrades UI Functions----------

    public void ReturnFromUpgrades()
    {
        Debug.Log("Returning from port Thames");
        upgradesUI.SetActive(false);
        portThamesUI.SetActive(true);
    }

    //----------Upgrades UI Functions----------
}
