using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerConnection : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] PlayerMovement pm;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("collided");
        if (other.tag == "Ship")
        {
            Debug.Log("ShipCollider");
            pm.SetParent(other.transform);
        }
    }


}
