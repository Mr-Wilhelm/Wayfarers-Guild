using Gravitas;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SCR_TerrainCollision : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private GravitasBody shipRB;
    [SerializeField] private float invincibilityPeriod = 1.0f; // Time in seconds that the ship is invincible after hitting terrain
    [SerializeField] bool terrainNormalsInverted = false; // If true, the terrain is considered to have inverted normals

    // Variables to store whether the ship has recently hit terrain
    private bool shipRecentlyHitTerrain;
    private bool shipInCollider;

    // Variables to store collision data
    private Vector3 collisionNormal;
    private Vector3 collisionPosition;

    //the velocity of the ship before the impact
    private Vector3 preImpactVelocity;

    //private void OnTriggerEnter(Collider collision)
    //{
    //    //Checks if collision was with ship
    //    if (!collision.gameObject.CompareTag("Ship")) { return; }
    //    shipInCollider = true;
    //
    //    //If the ship still in invicible stage then return
    //    if (shipRecentlyHitTerrain) { return; }
    //
    //
    //
    //    shipRB = collision.gameObject.GetComponent<GravitasBody>();
    //
    //    //Gets the collision normal and positon
    //    collisionNormal = contactPoint.normal; HOW DO I GET THIS???
    //    collisionPosition = collision.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
    //
    //    //reverts the normal if the terrain normals are inverted
    //    if (terrainNormalsInverted) { collisionNormal *= -1; }
    //    //Debug.Log("Hit recently set to : " + shipRecentlyHitTerrain);
    //
    //    //triggers the bounce or reflection from the collision
    //    Bounce(collisionNormal, collisionPosition);
    //    shipRecentlyHitTerrain = true;
    //    //Debug.Log("Hit recently set to : " + shipRecentlyHitTerrain);
    //    //Invoke("ResetShipHitTerrainRecently", invincibilityPeriod);
    //    StartCoroutine(ResetInvunerability(invincibilityPeriod));
    //}
    private void OnCollisionEnter(Collision collision)
    {
        // Checks if collision was with ship
        if (!collision.gameObject.CompareTag("Ship")) { return; }
        shipInCollider = true;
        
        //If the ship still in invicible stage then return
        if (shipRecentlyHitTerrain) { return; }

        preImpactVelocity = collision.relativeVelocity.normalized;
        //Debug.Log(preImpactVelocity);


        shipRB = collision.gameObject.GetComponent<GravitasBody>();

        //Gets the collision normal and position
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 collisionPoint = contactPoint.point;
        collisionNormal = contactPoint.normal.normalized;
        collisionPosition = contactPoint.point;

        //reverts the normal if the terrain normals are inverted
        if (terrainNormalsInverted) { collisionNormal *= -1; }
        //Debug.Log("Hit recently set to : " + shipRecentlyHitTerrain);

        //triggers the bounce or reflection from the collision
        Bounce(collisionNormal, collisionPosition);
        shipRecentlyHitTerrain = true;
        //Debug.Log("Hit recently set to : " + shipRecentlyHitTerrain);
        //Invoke("ResetShipHitTerrainRecently", invincibilityPeriod);
        StartCoroutine(ResetInvunerability(invincibilityPeriod));
    }

    private void OnCollisionExit(Collision collision)
    {
        //checks if the ship is not colliding with the terrain anymore
        if (!collision.gameObject.CompareTag("Ship")) { return; }
        shipInCollider = false;
    }

    /// <summary>
    /// Bounces the ship off the terrain
    /// </summary>
    /// <param name="colNormal">the normal of the the terrain</param>
    /// <param name="colPos">the position of the collision</param>
    private void Bounce(Vector3 colNormal, Vector3 colPos)
    {
        Debug.Log("Bounce triggered");
        //store the ships velocity before the collision
        
        //gets the distance from the collision to the point of the ship (to be used as a direction for the reflection since normal can be a bit funky) this will be normalised and used as a direction
        Vector3 distance = shipRB.gameObject.transform.position - colPos;

        //calculates the angle of collision to make sure the ship is not pushed into the terrain
        float collisionDotProduct = Vector3.Dot(preImpactVelocity.normalized, colNormal.normalized);
        Debug.Log("ColliderNormal: " + collisionNormal.normalized);
        Debug.Log("Before Impact Velocity: " + preImpactVelocity.normalized);
        Debug.Log($"{collisionDotProduct}");
        if (collisionDotProduct < 0)
        {
            //shipRB.AddForce(colNormal.normalized * bounceForce *10, ForceMode.Impulse);
            //Debug.Log("Normal Collision angle");
            Debug.DrawLine(colPos, colPos + (colNormal * 10), Color.red,50f);
            Debug.DrawLine(colPos +  (colNormal * 10), colPos + (colNormal * 10)+ Vector3.up,Color.red, 50f);
            
            Vector3 reflectionDir = Vector3.Reflect(preImpactVelocity.normalized, colNormal.normalized);
            Debug.Log("Reflection dir: " + reflectionDir);
            shipRB.AddForce(reflectionDir * bounceForce*100, ForceMode.Impulse);
        }

        // if the the reflection is calculated at a weird angle it will use the direction of the ship distance
        else
        {
            Debug.Log("Non normal angle do nothing");
            //if(beforeImpactVelocity.magnitude < 10) { beforeImpactVelocity = beforeImpactVelocity.normalized * 10; }
            //Vector3 inputDirAndRef = distance.normalized * beforeImpactVelocity.magnitude;
            //Vector3 reflection = Vector3.Reflect(inputDirAndRef*-1, colNormal);
            //
            //shipRB.AddForce((reflection.normalized * bounceForce), ForceMode.Impulse);
           

        }
    }


    /// <summary>
    /// Resets the ship's recently hit terrain status and checks whether its currently colliding with terrain.
    /// </summary>
    //private void ResetShipHitTerrainRecently()
    //{
    //    shipRecentlyHitTerrain = false;
    //
    //    if (shipInCollider)
    //    {
    //        if (terrainNormalsInverted) { collisionNormal *= -1; }
    //        Bounce(collisionNormal, collisionPosition);
    //    }
    //}

    IEnumerator ResetInvunerability(float Time)
    {
        //Debug.Log("Invulerablity on;");
        yield return new WaitForSeconds(Time);
        //Debug.Log("Invulnerability turned off");
        shipRecentlyHitTerrain = false ;

        if (shipInCollider)
        {
            if (terrainNormalsInverted) { collisionNormal *= -1; }
            Bounce(collisionNormal, collisionPosition);
        }

    }

}
