using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerNetworkManager : NetworkBehaviour
{
    [SerializeField] private GameObject networkLogicObject;

    [SerializeField] private GameObject networkLogicPrefab;

    [SerializeField] private Camera playerCamera;

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
            //    GameObject.Find("PlayerName").GetComponent<TextMeshPro>().text = GameObject.Find("PlayerDataHandler").GetComponent<PlayerDataHandler>().player1Name.Value;
            //}
            //else
            //{
            //    GameObject.Find("PlayerName").gameObject.GetComponent<TextMeshPro>().text = GameObject.Find("PlayerDataHandler").GetComponent<PlayerDataHandler>().player2Name.Value;
            //}
        }
    }
}
