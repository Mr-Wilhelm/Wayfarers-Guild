using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

//- Syncing player name script from:
//- Author: https://www.youtube.com/@rootbindev
//- Date published: 13 Jul 2024
//- Date accessed: 29 Sept 2024
//- Title of program/source code: Unity Multiplayer 2024, Syncing Player position and name!
//- Web address: https://www.youtube.com/watch?v=-ZKMV1lmORA&t=947s

public class SCR_PlayerSettings : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshPro playerName;
    [SerializeField] private TextMeshProUGUI playerNameBelow;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject Camera;
    [SerializeField] private GameObject networkManager;

    public GameObject CraigBody;
    public GameObject CraigClothes;
    public LayerMask SelfPlayerMesh;

    //gameobject for the ship to try parenting player to

    //Network variables are able to synch data between clients network string is a custom struct that can store player name, can set default which is unknown and read and write perms
    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> firstPlayerConnected = new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        
        networkManager = GameObject.FindGameObjectWithTag("NetworkManager");
        if (IsOwner)
        {
            //Gets the input of the input name text box from the UI manager
            if (string.IsNullOrWhiteSpace(networkPlayerName.Value))
            {
                networkPlayerName.Value = "Player 1";
                if (firstPlayerConnected.Value)
                {
                    networkPlayerName.Value = "Player 2";
                }
            }
            if (firstPlayerConnected.Value == false)
            {
                GameObject enemySpawner = GameObject.Find("SCR_EnemySpawner");
            }
            else
            {
                GameObject.Find("TEMPShip").GetComponent<SCR_ShipHealth>().SetHealthBarForSecondPlayer();
            }

            CraigClothes.layer = SelfPlayerMesh;//LayerMask.NameToLayer("SelfPlayerMesh");
            CraigBody.layer = SelfPlayerMesh;//LayerMask.NameToLayer("SelfPlayerMesh");


            //DO STARTING STUFF HERE WITH SHIPPLATFORM
            //Transform Airship = GameObject.FindGameObjectWithTag("PlayerAirShip").transform;
            //if (Airship != null)
            //{
            //    Debug.Log("Found Airship");
            //    //this.transform.parent = Airship;

            //    if (NetworkObject.TrySetParent(Airship.GetComponent<NetworkObject>(), false))
            //    {
            //        Debug.Log(transform.position);
            //        transform.localPosition = new Vector3(0,1.5f,0);
            //        Debug.Log(transform.position);

            //    }
            //    else
            //    {
            //        Debug.Log("Didnt work");
            //    }


            //    //this.transform.parent = Airship;


            //}
            //else
            //{
            //    Debug.Log("couldnt find airship or airship is null");
            //}
        }
        else
        {
            Camera.SetActive(false);
            this.tag = "Untagged";
            Destroy(this.gameObject.GetComponent<SCR_PlayerInteract>());
        }
        //Sets player name to string in case there were any numbers that mess it up
        playerName.text = networkPlayerName.Value.ToString();
        playerNameBelow.text = networkPlayerName.Value.ToString();
        networkPlayerName.OnValueChanged += OnNetworkPlayerName_OnValueChange;
        if (!IsOwner) { Canvas.SetActive(false); }
        Debug.Log("Player connecting");
        firstPlayerConnected.Value = true;
    }

    public void OnNetworkPlayerName_OnValueChange(NetworkString previousValue, NetworkString newValue)
    {
        
        playerName.text = newValue;
        //playerNameBelow.GetComponent<TextMeshPro>().SetText(newValue);
    }

    
}

public struct NetworkString : INetworkSerializeByMemcpy
{
    private ForceNetworkSerializeByMemcpy<FixedString32Bytes> _info;

    //Compressing into a value that can be sent over network, network serialize is called when trying to send/receive data over network
    public void NetworkSerialize<T>(BufferSerializer<T> serializer)
        where T : IReaderWriter
    {
        serializer.SerializeValue(ref _info);
    }

    //Taking compressed value sent over network and returns it to a string that is readable
    public override string ToString()
    {
        return _info.Value.ToString();
    }

    //Running To string function to return network data into readable string
    public static implicit operator string(NetworkString s) => s.ToString();

    //Assigns regular strings into network strings for sending across network
    public static implicit operator NetworkString(string s) => new NetworkString() { _info = new FixedString32Bytes(s) };
}



