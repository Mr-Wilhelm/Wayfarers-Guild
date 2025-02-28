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
        shipRB = collision.gameObject.GetComponent<GravitasBody>();
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 collisionPoint = contactPoint.point;
        //Debug.Log("Collision point is: " + collisionPoint);

        Vector3 collisionNormal = contactPoint.normal;

        Bounce(collisionNormal);
        shipRecentlyHitTerrain = true;
        Invoke("ResetShipHitTerrainRecently", invincibilityPeriod);
    }

    private void Bounce(Vector3 colNormal)
    {
        Vector3 InvertedNormal = colNormal * -1;
        Vector3 beforeImpactVelocity = shipRB.Velocity;
        float CollisionDotProduct = Vector3.Dot(beforeImpactVelocity, InvertedNormal);
        Debug.Log(InvertedNormal + " Normal");
        Debug.Log(beforeImpactVelocity + " ShipDir");
        Debug.Log(CollisionDotProduct + " Result");
        

        if (CollisionDotProduct < 0.25)
        {
            shipRB.AddForce(beforeImpactVelocity.normalized *-1 * bounceForce * 10 * beforeImpactVelocity.magnitude, ForceMode.Impulse);
            Debug.Log("Collision DOT product mag");
        }
        //else
        //{
        //    Debug.Log(beforeImpactVelocity);
        //    Vector3 reflection = Vector3.Reflect(beforeImpactVelocity, colNormal);
        //    Debug.Log("relection direction is: " + reflection);
        //    shipRB.AddForce((reflection * bounceForce), ForceMode.Impulse);
            
        //}
    }

    private void ResetShipHitTerrainRecently()
    {
        shipRecentlyHitTerrain = false;
    }
}
