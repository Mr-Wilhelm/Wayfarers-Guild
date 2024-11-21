using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    private bool readyPressed = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuestButtonPress()
    {
        Debug.Log("Quest button pressed");
    }

    public void BackButton()
    {
        Debug.Log("Back button pressed");
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
