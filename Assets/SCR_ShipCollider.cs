using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitas;

public class SCR_ShipCollider : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private GravitasBody shipRB;

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & terrainLayer) != 0)
        {
            //Vector3 collisionPoint = other.ClosestPoint(transform.position); // Point of collision
            Vector3 hitPoint = other.ClosestPoint(transform.position);
            Vector3 bounceDirection = hitPoint.normalized;

            shipRB.AddForce(bounceDirection * bounceForce, ForceMode.Impulse);
            Debug.Log("Hit terrain at : " + hitPoint);
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
