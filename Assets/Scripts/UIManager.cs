using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

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

    [SerializeField]
    private TMP_InputField IPAddress;

    [Header("Networking Stuff")]
    [SerializeField]
    private NetworkManager networkManager;

    [SerializeField]
    private UnityTransport unityTransport;

    private void Start()
    {
        //Lines for hosting and joining as a client
        hostButton.onClick.AddListener(() => OnHost());
        joinButton.onClick.AddListener(() => OnJoin());

        isOnTitleScreen = true;
        isOnMainMenuScreen = false;


        networkManager = GameObject.Find("NetworkManager").GetComponent<NetworkManager>();
        unityTransport = GameObject.Find("NetworkManager").GetComponent<UnityTransport>();

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


        isOnMainMenuScreen = false; isOnTitleScreen = false;
        menuScreen.SetActive(false); titleScreen.SetActive(false);

        //host set to 0.0.0.0, an open call.
        NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "0.0.0.0";

        NetworkManager.Singleton.StartHost();

        NetworkManager.Singleton.SceneManager.LoadScene("DemoScene", LoadSceneMode.Single);
    }

    private void OnJoin()
    {
        if (NetworkManager.Singleton.IsListening)
        {
            Debug.Log("Trying to connect please wait...");
            return;
        }

        if (IPAddress.text == "")
        {
            //set as localhost. If on a host with IPV4, then 127.0.0.1 wont work
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = "127.0.0.1";
        }

        else
        {
            //set address to IP Address variable
            NetworkManager.Singleton.GetComponent<UnityTransport>().ConnectionData.Address = IPAddress.text;
        }

        NetworkManager.Singleton.StartClient();
    }

    private void Update()
    {
        //switches screens depending on which bool values are set, done on one line because i like suffering
        if (isOnTitleScreen && !isOnMainMenuScreen)
        {
            titleScreen.SetActive(true); menuScreen.SetActive(false);

            if (Input.anyKey)
            {
                isOnTitleScreen = false;
                isOnMainMenuScreen = true;

            }

        }

        else if (!isOnTitleScreen && isOnMainMenuScreen)
        {
            titleScreen.SetActive(false); menuScreen.SetActive(true);
        }

        //unityTransport.ConnectionData.Address = IPAddress.text;
    }

}
