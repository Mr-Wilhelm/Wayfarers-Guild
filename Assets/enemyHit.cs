using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemyHit : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Ship"))
        {
            Debug.Log("Hitting ship");
            other.gameObject.transform.root.GetComponent<ShipHealth>().TakeDamage();
            Destroy(gameObject.transform.parent.gameObject);
        }
    }
}
