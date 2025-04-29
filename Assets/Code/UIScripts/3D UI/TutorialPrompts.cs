using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class TutorialPrompts : NetworkBehaviour
{
    [SerializeField]
    private GameUIScript gameUI;

    private void Start()
    {
        gameUI = GameObject.Find("MainUICanvas").GetComponent<GameUIScript>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameUI.playerIsInRange = true;
            if(gameObject.tag == "EnginePrompt")
            {
                gameUI.isInEnginePrompt = true; //check if the player is in the engine prompt
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        gameUI.playerIsInRange = false;
        gameUI.isInEnginePrompt = false;
    }
}
