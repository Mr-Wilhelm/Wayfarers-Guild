using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class enemyHit : NetworkBehaviour
{
    private GameObject networkManager;

    private void Start()
    {
        networkManager = GameObject.Find("NetworkManager");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(!IsServer)
        {   
            //dont do stuff if you're the client, stops stuff from happening twice
            return;
        }

        if (other.gameObject.tag == "Ship")
        {
            Debug.Log("Hitting ship");
            other.gameObject.transform.root.GetComponent<ShipHealth>().TakeDamage();
            transform.root.GetComponent<NetworkObject>().Despawn();
        }
    }
}
