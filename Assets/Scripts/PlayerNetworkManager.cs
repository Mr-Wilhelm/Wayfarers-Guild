using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    private GameObject networkManager;
    private GameObject playerOne;
    private GameObject playerTwo;

    /// <summary>
    /// On network spawn of players, fills player objects with each player
    /// </summary>
    public override void OnNetworkSpawn()
    {
        //Get network manager
        networkManager = GameObject.Find("NetworkManager");
        //Checks if script is attached to the player that is the owner
        if (IsOwner)
        {
            //Gets the player variables from the network logic script
            playerOne = networkManager.GetComponent<NetworkLogic>().playerOne;
            playerTwo = networkManager.GetComponent<NetworkLogic>().playerTwo;
            //If the player one slot has not been filled
            if (playerOne == null)
            {
                //Fill it with yourself (what I do to amanda)
                networkManager.GetComponent<NetworkLogic>().playerOne = this.gameObject;
                Debug.Log("Filled player one");
            }
            //If the player two slot has not been filled
            else if (playerTwo == null)
            {
                //Fill it with yourself (what I do to amanda)
                networkManager.GetComponent<NetworkLogic>().playerTwo = this.gameObject;
                Debug.Log("Filled player two");
            }
            else
            {
                //If both player slots have been filled
                Debug.Log("Both players full");
            }
            
        }
    }
}
