using Gravitas.Demo;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class SCR_PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private GameObject networkLogicObject;

    [SerializeField] private GameObject networkLogicPrefab;

    [SerializeField] private Camera playerCamera;

    [SerializeField] GameObject Ship = null;

    [SerializeField] public GameObject CraigBody;

    [SerializeField] public GameObject CraigClothes;

    [SerializeField] public LayerMask SelfPlayerMesh;

    //Tracks the pos and rot of the player
    public NetworkVariable<Vector3> playerPos = new NetworkVariable<Vector3>();
    public NetworkVariable<Vector3> playerRot = new NetworkVariable<Vector3>();

    public NetworkVariable<FixedString128Bytes> playerName = new NetworkVariable<FixedString128Bytes>();
    public string playerNameString;


    [SerializeField] private GameObject myPrefab;

    public override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= test;
    }


    /// <summary>
    /// On network spawn of players, fills player objects with each player
    /// </summary>
    /// 
    public override void OnNetworkSpawn()
    {
        if(SceneManager.GetActiveScene().name == "CityMenu")
        {
            test(SceneManager.GetActiveScene(), LoadSceneMode.Single);
        }
        SceneManager.sceneLoaded += test;


        playerName.OnValueChanged += OnNetworkPlayerName_OnValueChange;
        gameObject.name = playerName.Value.ToString();

        Debug.Log("GRAVITAS PLAYER SPAWNED");
        if (IsOwner)
        {

            FixedString128Bytes name = "Player_" + NetworkManager.Singleton.LocalClientId;
            updateNameServerRPC(name);

            Debug.Log("Owner detected");
            //enable camera for owner
            playerCamera = gameObject.GetComponentInChildren<Camera>();
            playerCamera.enabled = true;
            

            //Set each player's body mesh to self player mesh so they are not rendered by the player that owns them's camera
            int SelfPlayerMeshLayer = LayerMask.NameToLayer("SelfPlayerMesh");
            CraigBody.layer = SelfPlayerMeshLayer;
            CraigClothes.layer = SelfPlayerMeshLayer;
        }
        else
        {
            gameObject.GetComponent<GravitasFirstPersonPlayerSubject>().enabled = false;
            Debug.Log(playerNameString + " is not owner, disabling movement");
            playerCamera.enabled = false;
            int NonOwnerLayer = LayerMask.NameToLayer("NonOwnerLayer");
            gameObject.layer = NonOwnerLayer;
        }
    }

    void test(Scene a, LoadSceneMode b)
    {
        if (a.name == "CityMenu")
        {

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            //if (!myPrefab) { Debug.Log("Prefab Is Empty"); }
            //else
            //{
            //    spawnWithOwnershipServerRpc(OwnerClientId);

            //    Debug.Log("Empty Object");
            //}




            //NetworkObject.Despawn(true);
            //this.gameObject.SetActive(false);
            this.GetComponent<SCR_2D_Logic>().enabled = true;
            this.GetComponent<GravitasFirstPersonPlayerSubject>().enabled = false;
            this.enabled = false;

        }
    }

    //Late update happpens at the end of a frame
    private void Update()
    {
        //Update the network variables of pos and rot
        if (IsOwner)
        {
            updatePosServerRPC(transform.position);
            updateRotServerRPC(transform.rotation.eulerAngles);
        }
    }


    //Update the playername variable in the network
    [ServerRpc]
    private void updateNameServerRPC(FixedString128Bytes newName)
    {
        playerName.Value = newName;
    }

    //Change the player name variable and also update the game object name of each player to represent each of them
    private void OnNetworkPlayerName_OnValueChange(FixedString128Bytes previousValue, FixedString128Bytes newValue)
    {
        playerNameString = playerName.Value.ToString();
        gameObject.name = playerNameString;
    }

    //Updates the owner's pos and sends it to the network
    [ServerRpc]
    private void updatePosServerRPC(Vector3 newPos)
    {
        updatePosClientRPC(newPos);
    }

    //Updates the owner's rot and sends it to the network
    [ServerRpc]
    private void updateRotServerRPC(Vector3 newRot)
    {
        updateRotClientRPC(newRot);
    }

    [ClientRpc]
    private void updatePosClientRPC(Vector3 newPos)
    {
        if(!IsOwner)
        {
            transform.position = newPos;
        }
    }

    [ClientRpc]
    private void updateRotClientRPC(Vector3 newRot)
    {
        if (!IsOwner)
        {
            transform.rotation = Quaternion.Euler(newRot);
        }
    }


    [ServerRpc]
    private void spawnWithOwnershipServerRpc(ulong Id)
    {
        var instance = Instantiate(myPrefab);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.SpawnWithOwnership(Id);
    }

}
