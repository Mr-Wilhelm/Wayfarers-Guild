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

public class PlayerSettings : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private TextMeshPro playerName;
    [SerializeField] private TextMeshProUGUI playerNameBelow;
    [SerializeField] private GameObject Canvas;
    [SerializeField] private GameObject Camera;

    //Network variables are able to synch data between clients network string is a custom struct that can store player name, can set default which is unknown and read and write perms
    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> firstPlayerConnected = new NetworkVariable<bool>(false);
    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            //Gets the input of the input name text box from the UI manager
            networkPlayerName.Value = GameObject.Find("UIManager").GetComponent<UIManager>().nameInputField.text;
            if (networkPlayerName.Value == "")
            {
                networkPlayerName.Value = "Player 1";
                if (firstPlayerConnected.Value)
                {
                    networkPlayerName.Value = "Player 2";
                }
            }
            playerNameBelow.text = networkPlayerName.Value.ToString();
        }
        else
        {
            Camera.SetActive(false);
        }
        //Sets player name to string in case there were any numbers that mess it up
        playerName.text = networkPlayerName.Value.ToString();
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

