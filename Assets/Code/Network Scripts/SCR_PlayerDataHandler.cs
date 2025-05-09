using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_PlayerDataHandler : NetworkBehaviour
{
    public NetworkVariable<NetworkString> player1Name = new NetworkVariable<NetworkString>("Host", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<NetworkString> player2Name = new NetworkVariable<NetworkString>("Client", NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public NetworkVariable<float> shipHealthGlobal = new NetworkVariable<float>(100);
    public NetworkVariable<float> playerMoney = new NetworkVariable<float>(200);
    public NetworkVariable<float> shipArmour = new NetworkVariable<float>(0);   //TODO: Get ship overall armour and start health

    public int armourUpgradesBought = 0;
    public int healthUpgradesBought = 0;

    public float armourUpgradeIncrement = 0.5f;
    public float healthUpgradeIncrement = 10.0f;

    //public NetworkVariable<TextMeshProUGUI> playerMoneyText = new NetworkVariable<TextMeshProUGUI>();

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
       
    }
    //disable and re enable the object so that it can get reloaded when the scene loads
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    //Does stuff when the scene loads
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("New Scene Loaded: " + scene.name);
        Debug.Log("Decrease value from armour is " + GetDecreaseFromArmour().Value);

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

    //TODO make sure this works and actually sets the names correctly
    //TODO Figure out where to call this function, ideally after host is pressed, and before scene is switched
    [ServerRpc(RequireOwnership = false)]
    public void UpdatePlayerNameServerRPC()
    {
        TMP_InputField NameInputField = GameObject.Find("Name").GetComponent<TMP_InputField>();
        if (NameInputField.text == "Name" || NameInputField.text == "")
        {
            if (IsHost) { player1Name.Value = "Player Host"; }
            else if (IsClient) { player2Name.Value = "Player Client"; }
        }

        if (IsHost)
        {
            player1Name.Value = NameInputField.text;
        }
        else if (IsClient)
        {
            player2Name.Value = NameInputField.text;
        }
    }

    //calculate the damage reduction from total armour
    public NetworkVariable<float> GetDecreaseFromArmour()
    {
        return new NetworkVariable<float>(armourUpgradesBought * armourUpgradeIncrement);
    }
    public NetworkVariable<float> GetTotalHealth()
    {
        return new NetworkVariable<float>(shipHealthGlobal.Value + (healthUpgradeIncrement * healthUpgradesBought));
    }
}
