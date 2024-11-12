using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerDataHandler : NetworkBehaviour
{

    public NetworkVariable<NetworkString> player1Name = new NetworkVariable<NetworkString>("Player 1", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<NetworkString> player2Name = new NetworkVariable<NetworkString>("Player 2", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
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

}
