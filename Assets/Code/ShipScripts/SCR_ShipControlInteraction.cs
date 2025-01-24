using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShipControlInteraction : SCR_InteractableScript
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInteractScript.canSteerShip = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInteractScript.canSteerShip = false;
        }
    }
}
