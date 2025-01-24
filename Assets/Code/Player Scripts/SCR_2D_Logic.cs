using Gravitas.Demo;
using NUnit.Framework.Internal;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SCR_2D_Logic : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        
        SceneManager.sceneLoaded += test;

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= test;
    }

    void test(Scene a, LoadSceneMode b)
    {
        if (a.name == "SCN_DemoScene")
        {

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            

            this.GetComponent<SCR_PlayerNetworkManager>().enabled = true;
            this.GetComponent<GravitasFirstPersonPlayerSubject>().enabled = true;
            this.GetComponent<SCR_ShipControls>().enabled = true;
            transform.position = GameObject.Find("PRE-Airship").transform.position;
            this.enabled = false;

        }
    }


}
