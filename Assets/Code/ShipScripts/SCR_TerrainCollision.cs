using Gravitas;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
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

    private void OnCollisionEnter(Collision collision)
    {
        // Checks if collision was with ship
        if (!collision.gameObject.CompareTag("Ship")) { return; }
        shipInCollider = true;
        
        //If the ship still in invicible stage then return
        if (shipRecentlyHitTerrain) { return; }


        //the velocity of the ship before the impact 
        preImpactVelocity = collision.relativeVelocity;
       

        
        shipRB = collision.gameObject.GetComponent<GravitasBody>();

        //Gets the collision normal and position
        ContactPoint contactPoint = collision.contacts[0];
        collisionNormal = contactPoint.normal.normalized;
        collisionPosition = contactPoint.point;

        //reverts the normal if the terrain normals are inverted
        if (terrainNormalsInverted) { collisionNormal *= -1; }
     

        //triggers the bounce or reflection from the collision
        Bounce(collisionNormal, collisionPosition);
        
        //Sets bool for Incinvbility period after collisions
        shipRecentlyHitTerrain = true;
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
      

        //calculates the angle of collision to make sure the ship is not pushed into the terrain
        float collisionDotProduct = Vector3.Dot(preImpactVelocity.normalized, colNormal.normalized);

        //Debug.Log("Before Impact Velocity: " + preImpactVelocity.normalized);
        //Debug.Log("ColliderNormal: " + collisionNormal.normalized);
        //Debug.Log($"{collisionDotProduct}");

        //calculates if the ship is heading towards or away from the colliding normal
        if (collisionDotProduct < 0)
        {
            //debug Functions that draw the normal of the colliding surface
            //Debug.DrawLine(colPos, colPos + (colNormal * 10), Color.red,50f);
            //Debug.DrawLine(colPos +  (colNormal * 10), colPos + (colNormal * 10)+ Vector3.up,Color.red, 50f);

            //calculate the reflection angle and adds force
            Vector3 reflectionDir = Vector3.Reflect(preImpactVelocity.normalized, colNormal.normalized);
            //shipRB.AddForce(reflectionDir * bounceForce * preImpactVelocity.magnitude, ForceMode.Impulse);
            shipRB.gameObject.GetComponent<Rigidbody>().AddForceAtPosition(reflectionDir * bounceForce * preImpactVelocity.magnitude, colPos,ForceMode.Impulse);

            TakeShipDamage();

            //TODO make the ship change angle based on how it collided

            



        }


    }

    /// <summary>
    /// Coroutine that handles the timer for the invinciblity period
    /// </summary>
    /// <param name="Time"> the amount of time that the ship should be invincible for</param>
    /// <returns></returns>
    IEnumerator ResetInvunerability(float Time)
    {
        yield return new WaitForSeconds(Time);

        shipRecentlyHitTerrain = false ;

        if (shipInCollider)
        {
            if (terrainNormalsInverted) { collisionNormal *= -1; }
            Bounce(collisionNormal, collisionPosition);
        }

    }


    /// <summary>
    /// TODO DamageFunction
    /// </summary>
    public void TakeShipDamage()
    {
        //MAKE SHIP TAKE DAMAGE HERE OR
    }

}
