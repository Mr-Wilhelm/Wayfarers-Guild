using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private GameObject networkLogicObject;

    [SerializeField] private NetworkLogic networkLogic;

    [SerializeField] private GameObject networkLogicPrefab;

    /// <summary>
    /// On network spawn of players, fills player objects with each player
    /// </summary>
    /// 
    public override void OnNetworkSpawn()
    {
        if(GameObject.Find("NetworkLogicObj") == null)
        {
            var networkLogicVar = Instantiate(networkLogicPrefab);
            DontDestroyOnLoad(networkLogicVar);
            var instanceNetworkObject = networkLogicVar.GetComponent<NetworkObject>();
            instanceNetworkObject.Spawn(true);
        }
        //Get network manager
        networkLogicObject = GameObject.FindGameObjectWithTag("NetworkLogicObject");

        //get network logic script
        networkLogic = networkLogicObject.GetComponent<NetworkLogic>();
        //Checks if script is attached to the player that is the owner
        if (IsOwner)
        {
            //If the player one slot has not been filled
            if (networkLogic.playerOneGameObj == null)
            {
                //Fill it with yourself (what I do to amanda)
                //networkLogic.playerOne.Value = this.gameObject;
                networkLogic.SendPlayerOneObjectRpc(this.gameObject);
                Debug.Log("Filled player one");
                networkLogic.owner = 1;
            }
            //If the player two slot has not been filled
            else if (networkLogic.playerTwoGameObj == null)
            {
                //Fill it with yourself (what I do to amanda)
                //networkLogic.playerTwo.Value = this.gameObject;
                networkLogic.SendPlayerTwoObjectRpc(this.gameObject);
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
