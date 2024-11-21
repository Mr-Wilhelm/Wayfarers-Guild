using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    private bool readyPressed = false;

    public void QuestButtonPress()
    {
        Debug.Log("Quest button pressed");
    }

    public void BackButton()
    {
        Debug.Log("Back button pressed");
    }

    public void PortThamesButton()
    {
        Debug.Log("Port Thames button pressed");
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

}
