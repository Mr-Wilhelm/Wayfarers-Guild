using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_EnemyDespawner : MonoBehaviour
{
    // Start is called before the first frame update
    public void DespawnEnemy(GameObject enemy)
    {
        if (NetworkManager.Singleton.IsHost)
        {
            enemy.GetComponent<NetworkObject>().Despawn();
            Debug.Log("Despawning enemy from network manager");
        }
    }

}
