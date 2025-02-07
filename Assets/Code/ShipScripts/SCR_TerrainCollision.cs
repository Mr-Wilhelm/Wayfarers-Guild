using Gravitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TerrainCollision : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private GravitasBody shipRB;
    [SerializeField] private float invincibilityPeriod = 1.0f;
    private bool shipRecentlyHitTerrain;

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Ship") || shipRecentlyHitTerrain) { return; }
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 collisionPoint = contactPoint.point;
        Debug.Log("Collision point is: " + collisionPoint);

        Vector3 collisionNormal = contactPoint.normal;

        Bounce(collisionNormal);
        shipRecentlyHitTerrain = true;
        Invoke("ResetShipHitTerrainRecently", invincibilityPeriod);
    }

    private void Bounce(Vector3 colNormal)
    {
        Vector3 beforeImpactVelocity = shipRB.Velocity;
        Vector3 reflection = Vector3.Reflect(beforeImpactVelocity, colNormal);
        Debug.Log("relection direction is: " + reflection);
        shipRB.AddForce((reflection * bounceForce), ForceMode.Impulse);
    }

    private void ResetShipHitTerrainRecently()
    {
        shipRecentlyHitTerrain = false;
    }
}
