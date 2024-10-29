using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

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
    public NetworkVariable<GameObject> playerOne = new NetworkVariable<GameObject>
        (null,  //new network variable type
        NetworkVariableReadPermission.Everyone, //read permission
        NetworkVariableWritePermission.Owner    //write permission
        );

    public NetworkVariable<GameObject> playerTwo = new NetworkVariable<GameObject>
        (null, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Owner
        );

    [SerializeField] private GameObject Camera;


    //clientside int to check which player is owner
    public int owner;

    /// <summary>
    /// Disabling all necessary components for the non owner
    /// Based on the owner variable
    /// </summary>

    public void Awake()
    {
        DontDestroyOnLoad(this);
    }

    [Rpc(SendTo.Everyone)]
    public void DisableNonOwnerRpc()
    {
        if (owner == 1)
        {
            playerTwo.Value.transform.Find("CameraHolder/Camera").gameObject.SetActive(false);
            playerTwo.Value.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Disabled player 2");
        }


        else if (owner == 2)
        {
            playerOne.Value.transform.Find("CameraHolder/Camera").gameObject.SetActive(false);
            playerOne.Value.GetComponent<PlayerMovement>().enabled = false;
            Debug.Log("Disabled player 1");
        }
    }
}
