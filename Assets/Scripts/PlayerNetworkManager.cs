using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private GameObject networkManager;

    [SerializeField] private NetworkLogic networkLogic;

    /// <summary>
    /// On network spawn of players, fills player objects with each player
    /// </summary>
    /// 
    public override void OnNetworkSpawn()
    {
        //Get network manager
        networkManager = GameObject.Find("NetworkManager");

        //get network logic script
        networkLogic = NetworkManager.GetComponent<NetworkLogic>();
        //Checks if script is attached to the player that is the owner
        if (IsOwner)
        {
            //If the player one slot has not been filled
            if (networkLogic.playerOne.Value == null)
            {
                //Fill it with yourself (what I do to amanda)
                networkLogic.playerOne.Value = this.gameObject;
                Debug.Log("Filled player one");
                networkLogic.owner = 1;
            }
            //If the player two slot has not been filled
            else if (networkLogic.playerTwo.Value == null)
            {
                //Fill it with yourself (what I do to amanda)
                networkLogic.playerTwo.Value = this.gameObject;
                Debug.Log("Filled player two");

                networkLogic.owner = 2;

                networkLogic.DisableNonOwnerRpc();
            }
            else
            {

                //If both player slots have been filled
                Debug.Log("Both players full");
            }
            
        }
    }
}
