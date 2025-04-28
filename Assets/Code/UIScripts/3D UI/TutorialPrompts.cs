using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

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
        }
    }
    private void OnTriggerExit(Collider other)
    {
        gameUI.playerIsInRange = false;
    }
}
