using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] public TMP_InputField nameInputField;
    private bool hostPressed;

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
        hostPressed = true;
    }

    private void OnJoin()
    {
        if (hostPressed)
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
