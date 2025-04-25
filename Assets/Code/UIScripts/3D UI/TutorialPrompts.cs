using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TutorialPrompts : NetworkBehaviour
{
    [SerializeField]
    private bool playerIsInRange;

    private void Start()
    {
        gameObject.transform.rotation = Quaternion.identity;
        gameObject.GetComponentInChildren<Canvas>().enabled = false;
    }
    private void Update()
    {
        if (playerIsInRange)
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
        if(other.gameObject.tag == "Player" && other.gameObject.GetComponentInChildren<Camera>().enabled)
        {
            gameObject.GetComponentInChildren<Canvas>().enabled = true;
            playerIsInRange = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            gameObject.GetComponentInChildren<Canvas>().enabled = false;
            playerIsInRange = false;
        }
    }
}
