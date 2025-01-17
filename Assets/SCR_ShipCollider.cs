using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_ShipCollider : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private Rigidbody shipRB;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & terrainLayer) != 0)
        {
            //Vector3 collisionPoint = other.ClosestPoint(transform.position); // Point of collision
            Vector3 bounceDirection = (transform.position - other.transform.position).normalized;

            shipRB.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            Debug.Log("Hit terrain");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == terrainLayer)
        {
            Debug.Log("Leaving terrain");
        }
    }
}
