using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipSpeedInteraction : InteractableScript
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInteractScript.canDoSpeedMinigame = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInteractScript.canDoSpeedMinigame = false;
        }
    }
}
