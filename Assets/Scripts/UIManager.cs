using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : NetworkBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] public TMP_InputField nameInputField;
    NetworkVariable<bool> hostPressed = new NetworkVariable<bool>(true);

    private void Start()
    {
        //Lines for hosting and joining as a client
        hostButton.onClick.AddListener(() => OnHost()); 
        joinButton.onClick.AddListener(() => OnJoin());
    }

    private void OnHost()
    {
        Debug.Log("Hosting");
        NetworkManager.Singleton.StartHost();
        hostPressed.Value = true;
    }

    private void OnJoin()
    {
        if (hostPressed.Value)
        {
            Debug.Log("Client connecting");
            NetworkManager.Singleton.StartClient();
        }
        else
        {
            Debug.Log("Host must connect first");
        }
    }
}
