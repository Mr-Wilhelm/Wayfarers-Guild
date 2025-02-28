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
        Vector3 collisionPosition = contactPoint.point;
        Bounce(collisionNormal*-1,collisionPosition);
        shipRecentlyHitTerrain = true;
        Invoke("ResetShipHitTerrainRecently", invincibilityPeriod);
    }

    private void Bounce(Vector3 colNormal, Vector3 colPos)
    {
        Vector3 beforeImpactVelocity = shipRB.Velocity;
        Vector3 dis = shipRB.gameObject.transform.position - colPos;
        float CollisionDotProduct = Vector3.Dot(beforeImpactVelocity, colNormal);
        Debug.Log(colNormal + " Normal");
        Debug.Log(beforeImpactVelocity + " ShipDir");
        Debug.Log(CollisionDotProduct + " Result");
        


        if (CollisionDotProduct > -0)
        {
            shipRB.AddForce(colNormal.normalized * bounceForce* 10, ForceMode.Impulse);
            Debug.Log("Collision DOT product mag");
        }
        else
        {
            if(beforeImpactVelocity.magnitude < 10) { beforeImpactVelocity = beforeImpactVelocity.normalized * 10; }
            Vector3 inputDirAndRef = dis.normalized * beforeImpactVelocity.magnitude;
            Vector3 reflection = Vector3.Reflect(inputDirAndRef*-1, colNormal);
            Debug.Log("relection direction is: " + reflection);
            shipRB.AddForce((reflection * bounceForce), ForceMode.Impulse);

        }
    }

    private void ResetShipHitTerrainRecently()
    {
        shipRecentlyHitTerrain = false;
    }
}
