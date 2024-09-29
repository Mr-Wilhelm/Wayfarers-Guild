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
    [SerializeField] private TextMeshPro playerNameBelow;
    private GameObject Canvas;

    //Network variables are able to synch data between clients network string is a custom struct that can store player name, can set default which is unknown and read and write perms
    NetworkVariable<NetworkString> networkPlayerName = new NetworkVariable<NetworkString>("Unknown", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Start()
    {
        //playerNameBelow = GameObject.Find("/Canvas/PlayerName").GetComponent<TextMeshPro>();
    }

    public override void OnNetworkSpawn()
    {
        Canvas = GameObject.Find("Canvas");
        playerNameBelow = Canvas.GetComponentInChildren<TextMeshPro>();
        Debug.Log(Canvas.name);
        Debug.Log("Player connecting");
        if (IsOwner)
        {
            //Gets the input of the input name text box from the UI manager
            networkPlayerName.Value = GameObject.Find("UIManager").GetComponent<UIManager>().nameInputField.text;
        }
        //Sets player name to string in case there were any numbers that mess it up
        playerName.text = networkPlayerName.Value.ToString();
        playerNameBelow.text = "jeff";
        //when player name is changed update the text to display it
        networkPlayerName.OnValueChanged += OnNetworkPlayerName_OnValueChange;
    }

    public void OnNetworkPlayerName_OnValueChange(NetworkString previousValue, NetworkString newValue)
    {
        
        playerName.text = newValue;
        playerNameBelow.GetComponent<TextMeshPro>().SetText(newValue);
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

