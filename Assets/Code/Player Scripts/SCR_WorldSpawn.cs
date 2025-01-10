using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SCR_WorldSpawn : NetworkBehaviour
{
    public GameObject world;
    public Vector3 spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        GameObject instantiatedWorld = Instantiate(world, spawnPoint, Quaternion.identity);
        NetworkObject networkInstantiatedWorld = instantiatedWorld.GetComponent<NetworkObject>();
        networkInstantiatedWorld.Spawn(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
