using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipControlInteraction : InteractableScript
{
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            playerInteractScript.canSteerShip = true;
            Debug.Log("beep");
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
