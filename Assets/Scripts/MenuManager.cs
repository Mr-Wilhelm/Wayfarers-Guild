using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject cityUI;

    [SerializeField]
    private GameObject portThamesUI;


    private bool readyPressed = false;

    private void Start()
    {
        cityUI = GameObject.Find("CityElements");
        cityUI.SetActive(true);

        portThamesUI = GameObject.Find("PortThames");   //finds the parent object with all the UI elements childed
        portThamesUI.SetActive(false);
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

    //-----------PORT THAMES UI STUFF------------

    public void PortThamesButton()
    {
        Debug.Log("Port Thames button pressed");
        portThamesUI.SetActive(true);
        cityUI.SetActive(false);
    }

    public void ReturnFromThames()
    {
        Debug.Log("Returning from port Thames");
        portThamesUI.SetActive(false);
        cityUI.SetActive(true);
    }

    public void ThamesRepair()
    {
        Debug.Log("Repair Ship");
    }

}
