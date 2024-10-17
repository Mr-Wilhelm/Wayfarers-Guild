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
        if (other.gameObject.tag == "Ship")
        {
            Debug.Log("Hitting ship");
            other.gameObject.transform.root.GetComponent<ShipHealth>().TakeDamage();
            networkManager.GetComponent<EnemyDespawner>().DespawnEnemy(gameObject.transform.root.gameObject);
        }
    }
}
