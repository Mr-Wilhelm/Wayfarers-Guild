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
    }

    private void OnJoin()
    {
        try
        {
            NetworkManager.Singleton.StartClient();
        }
        catch
        {
            Debug.Log("Balls");
        }
    }
}
