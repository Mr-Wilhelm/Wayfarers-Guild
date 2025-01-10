using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class SpawningPlatform : MonoBehaviour
{
    [SerializeField] GameObject Platform = null;
    // Start is called before the first frame update
    void Start()
    {
         
    }

    public void SpawnPlatform()
    {
        if (Platform != null)
        {
            GameObject PlatformGO = Instantiate(Platform);
            var PlatformGONetworkOBJ = PlatformGO.GetComponent<NetworkObject>();
            PlatformGONetworkOBJ.Spawn();
        }
    }



    
}
