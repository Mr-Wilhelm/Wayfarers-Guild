using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TutorialPrompts : NetworkBehaviour
{
    [SerializeField]
    private bool playerIsInRange;

    [SerializeField]
    private SCR_NewInteract interact;

    private void Start()
    {
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.GetComponentInChildren<Canvas>().enabled = false;

        if (IsOwner)
        {
            interact = GameObject.Find("Player_0").GetComponent<SCR_NewInteract>();
        }
    }
    private void Update()
    {
        if (interact == null)
        {
            interact = GameObject.Find("Player_1").GetComponent<SCR_NewInteract>();
        }

        if (interact.interacting)
        {
            Debug.Log("Hiding Prompt");
            gameObject.GetComponentInChildren<Canvas>().enabled = false;
        }

        if (gameObject.tag == "EnginePrompt" && playerIsInRange)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponentInChildren<Camera>().enabled && interact.objectBeingHeld == "Engine Food")
                {
                    gameObject.GetComponentInChildren<Canvas>().transform.LookAt(player.GetComponentInChildren<Camera>().transform.position);
                }
                else
                {
                    Debug.Log("No Showy showy");
                }
            }
        }
        else if (playerIsInRange && gameObject.tag != "EnginePrompt")
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                if (player.GetComponentInChildren<Camera>().enabled)
                {
                    gameObject.GetComponentInChildren<Canvas>().transform.LookAt(player.GetComponentInChildren<Camera>().transform.position);
                }
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.tag == "EnginePrompt" && other.gameObject.tag == "Player" && other.gameObject.tag == "Player" && other.gameObject.GetComponentInChildren<Camera>().enabled && interact.objectBeingHeld == "Engine Food")
        {
            gameObject.GetComponentInChildren<Canvas>().enabled = true;
            playerIsInRange = true;
        }

        if (other.gameObject.tag == "Player" && other.gameObject.GetComponentInChildren<Camera>().enabled)
        {
            if (gameObject.tag == "EnginePrompt" && interact.objectBeingHeld == "Engine Food")
            {
                gameObject.GetComponentInChildren<Canvas>().enabled = true;
                playerIsInRange = true;
            }
            else if (gameObject.tag == "EnginePrompt" && interact.objectBeingHeld != "Engine Food")
            {
                return;
            }
            else
            {
                gameObject.GetComponentInChildren<Canvas>().enabled = true;
                playerIsInRange = true;
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            gameObject.GetComponentInChildren<Canvas>().enabled = false;
            playerIsInRange = false;
        }
    }
}
