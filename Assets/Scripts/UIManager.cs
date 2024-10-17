using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.Netcode;

public class UIManager : MonoBehaviour
{
    [Header("Interactables")]
    [SerializeField] Button hostButton;
    [SerializeField] Button joinButton;
    [SerializeField] public TMP_InputField nameInputField;

    [Header("ScreenStuffs")]
    [SerializeField]
    private GameObject titleScreen;

    [SerializeField]
    private GameObject menuScreen;

    [SerializeField]
    private bool isOnTitleScreen;

    [SerializeField]
    private bool isOnMainMenuScreen;

    private void Start()
    {
        //Lines for hosting and joining as a client
        hostButton.onClick.AddListener(() => OnHost());
        joinButton.onClick.AddListener(() => OnJoin());

    }

    void clientDidThings()
    {
        Debug.Log("No server");

    }

    private void OnHost()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Trying to host please wait...");
            return;
        }
        NetworkManager.Singleton.StartHost();
    }

    private void OnJoin()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Trying to connect please wait...");
            return;
        }

        NetworkManager.Singleton.StartClient();
    }

    private void Update()
    {
        if (isOnTitleScreen && !isOnMainMenuScreen)
        {
            titleScreen.SetActive(true); menuScreen.SetActive(false);

        }

        else if(!isOnTitleScreen && isOnMainMenuScreen)
        {
            titleScreen.SetActive(false); menuScreen.SetActive(true);
        }
    }

}
