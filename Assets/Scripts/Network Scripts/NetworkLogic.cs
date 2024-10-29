using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.Collections;

//TODO: Make a new singleton called something, and add this script to that

/// <summary>
/// Other networking happening in:
///     -PlayerNetworkManager.csc
/// </summary>
public class NetworkLogic : NetworkBehaviour
{
    //make a singleton
    //public static NetworkLogic networkLogicSingleton { get; private set; }

    //im not sorry for setting up my variables like this :P
    //public NetworkVariable<NetworkString> playerOne = new NetworkVariable<NetworkString>
    //    ("",  //new network variable type
    //    NetworkVariableReadPermission.Everyone, //read permission
    //    NetworkVariableWritePermission.Owner    //write permission
    //    );

    //public NetworkVariable<NetworkString> playerTwo = new NetworkVariable<NetworkString>
    //    ("", 
    //    NetworkVariableReadPermission.Everyone, 
    //    NetworkVariableWritePermission.Owner
    //    );

    [SerializeField] public GameObject playerOneGameObj;
    [SerializeField] public GameObject playerTwoGameObj;
    [SerializeField] private GameObject Camera;


    //clientside int to check which player is owner
    public int owner;

    /// <summary>
    /// Disabling all necessary components for the non owner
    /// Based on the owner variable
    /// </summary>


    [Rpc(SendTo.Everyone)]
    public void DisableNonOwnerRpc()
    {
        //playerOneGameObj = GameObject.Find(playerOne.Value);
        //playerTwoGameObj = GameObject.Find(playerTwo.Value);

        if (owner == 1)
        {
            playerTwoGameObj.transform.Find("CameraHolder/Camera").gameObject.SetActive(false);
            playerTwoGameObj.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Disabled player 2");
        }


        else if (owner == 2)
        {
            playerOneGameObj.transform.Find("CameraHolder/Camera").gameObject.SetActive(false);
            playerOneGameObj.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Disabled player 1");
        }
    }

    [Rpc(SendTo.Everyone)]
    public void SendPlayerOneObjectRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            Debug.Log("target object received: " + targetObject.gameObject.transform.name);
            playerOneGameObj = targetObject.gameObject;
        }
        else
        {
            Debug.Log("Cumcum beans");
        }
    }
    [Rpc(SendTo.Everyone)]
    public void SendPlayerTwoObjectRpc(NetworkObjectReference target)
    {
        if (target.TryGet(out NetworkObject targetObject))
        {
            Debug.Log("target object received: " + targetObject.gameObject.transform.name);
            playerTwoGameObj = targetObject.gameObject;
        }
        else
        {
            Debug.Log("Cumcum beans");
        }
    }
}
