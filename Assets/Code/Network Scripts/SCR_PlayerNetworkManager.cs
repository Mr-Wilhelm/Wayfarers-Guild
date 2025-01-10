using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class SCR_PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private GameObject networkLogicObject;

    [SerializeField] private GameObject networkLogicPrefab;

    [SerializeField] private Camera playerCamera;

    [SerializeField] GameObject Ship = null;

    [SerializeField] public GameObject CraigBody;

    [SerializeField] public GameObject CraigClothes;

    [SerializeField] public LayerMask SelfPlayerMesh;

    /// <summary>
    /// On network spawn of players, fills player objects with each player
    /// </summary>
    /// 
    public override void OnNetworkSpawn()
    {

        if (IsOwner)
        {
            //enable camera for owner
            playerCamera = gameObject.GetComponentInChildren<Camera>();
            playerCamera.enabled = true;
            //if (IsHost)
            //{
            //    GameObject.Find("PlayerName").GetComponent<TextMeshPro>().text = GameObject.Find("SCR_PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>().player1Name.Value;
            //}
            //else
            //{
            //    GameObject.Find("PlayerName").gameObject.GetComponent<TextMeshPro>().text = GameObject.Find("SCR_PlayerDataHandler").GetComponent<SCR_PlayerDataHandler>().player2Name.Value;
            //}

            //Set each player's body mesh to self player mesh so they are not rendered by the player that owns them's camera
            Debug.Log("ACCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCCC");
            int SelfPlayerMeshLayer = LayerMask.NameToLayer("SelfPlayerMesh");
            CraigBody.layer = SelfPlayerMeshLayer;
            CraigClothes.layer = SelfPlayerMeshLayer;

        }
        Debug.Log("NetworkSpawn");
        SetParentToShip(true);
    }
    private void Update()
    {
        //need to find better place to put this
        SetParentToShip(false);

    }

    /// <summary>
    /// Setparent to the ship variable
    /// problem is when to call it ideally as soon as the network starts but calling it to early makes it not work
    /// put in update and it works put in start and it doesn work onnetwrokspawndoesnt get called??
    /// Debugs messages controlls whether to spam debug.log or not but also hides the not working error catch
    /// </summary>
    public void SetParentToShip(bool DebugsMessages)
    {
        if (DebugsMessages)
        {
            Debug.Log("calling");

        }
        if (Ship == null)
        {
            Ship = GameObject.FindWithTag("Ship");

        }
        try
        {
            NetworkObject.TrySetParent(Ship.transform, false);
            //if transform doesnt work set to gameobject instead?
        }
        catch
        {
            if (DebugsMessages)
            {
                Debug.Log("not wokring");
            }
        }
    }
}
