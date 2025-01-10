using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.NetworkInformation;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.VisualScripting;
using UnityEngine;
using AddressFamily = System.Net.Sockets.AddressFamily;



public class SCR_LANHostingAndConnecting : NetworkBehaviour
{
    protected string IPAddress;
    [SerializeField] GameObject IPTextBarOBJ;
    [SerializeField] GameObject IPInputFieldOBJ;


    [SerializeField] GameObject networkManagerOBJ;

    [SerializeField] NetworkManager networkManager;
    [SerializeField] UnityTransport unityTransport;


    private void Start()
    {
        //gets the IPV4 address and sets it to IPAddress string
        IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (IPAddress ip in hostEntry.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                IPAddress = ip.ToString();
                break;
            }
        }



        //gets the network manager script and unitytransport script
        networkManager = networkManagerOBJ.GetComponent<NetworkManager>();
        unityTransport = networkManagerOBJ.GetComponent<UnityTransport>();

    }



    public void HostAndDispalyIPV4()
    {
        //i think this is how i would start server?? TODO might need to be changed?
        unityTransport.StartServer();


        //displays the ip on the screen
        string IPText = "HostIP: "+ IPAddress;
        IPTextBarOBJ.GetComponent<TMP_Text>().text = IPText;
    }

    //coppies the IPV4 adress to clipboard
    public void CopyToClipboard()
    {
        TextEditor te = new TextEditor(); te.text = IPAddress; te.SelectAll(); te.Copy();
    }

    //i think this should make the player connect
    public void PressJoinButton()
    {
        
        TMP_InputField Field = IPInputFieldOBJ.GetComponent<TMP_InputField>();
        try
        {
            unityTransport.ConnectionData.Address = Field.text;
            Debug.Log("wrote IP " + Field.text + " address to the unity transport");

            //write connection stuff here TODO
        }
        catch (Exception e)
        {
            print("Error " + e);
            Field.text = "Error might be invalid IP adress";
        }
    }

    //get set IPADRES
    public string IPADDRESS
    {
        get { return IPAddress.ToString(); }

        set { IPAddress = value; }
    }
}
