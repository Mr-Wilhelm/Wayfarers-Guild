using Gravitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_Airship_Logic : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            player.GetComponent<SCR_PlayerNetworkManager>().bust4();
        }
    }

    private void OnDestroy()
    {
        GetComponent<GravitasField>().UnloadPhysicsScene();
    }
    public void killField()
    {
        GetComponent<GravitasField>().UnloadPhysicsScene();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
