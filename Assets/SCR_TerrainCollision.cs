using Gravitas;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_TerrainCollision : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private GravitasBody shipRB;
    Vector3 desiredForward = Vector3.zero;
    [SerializeField] private float invincibilityPeriod = 1.0f;
    private bool shipRecentlyHitTerrain;

    // Start is called before the first frame update
    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Collision detected with root: " + collision.transform.root.name);

    //    if (collision.gameObject.CompareTag("Ship"))
    //    {
    //        Debug.Log("piss pog");
    //    }
    //}

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

        // Calculate the desired direction the ship should face
        Vector3 desiredForward = reflection.normalized;
    }

    private void FixedUpdate()
    {
        if (desiredForward != Vector3.zero)
        {
            Vector3 currentForward = shipRB.transform.gameObject.transform.forward; // Current direction the ship is facing
            Vector3 rotationAxis = Vector3.Cross(currentForward, desiredForward); // Axis of rotation
            float angle = Vector3.Angle(currentForward, desiredForward); // Angle to rotate

            // If there's a significant difference, apply torque to rotate the ship
            if (angle > 1f)  // Check if the angle is significant enough to apply torque
            {
                // Apply torque based on this difference (apply some scaling factor for torque strength)
                float torqueStrength = angle * 100.0f; // Adjust torque strength as needed

                // Apply torque to rotate around the axis
                shipRB.AddTorque(rotationAxis * torqueStrength, ForceMode.Force);
            }
        }
    }

    private void ResetShipHitTerrainRecently()
    {
        shipRecentlyHitTerrain = false;
    }
}
