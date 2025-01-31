using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gravitas;

public class SCR_ShipCollider : MonoBehaviour
{
    [SerializeField] public LayerMask terrainLayer;
    [SerializeField] private float bounceForce = 10f;
    [SerializeField] private GravitasBody shipRB;
    Vector3 desiredForward = Vector3.zero;

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.contacts[0];
        Vector3 collisionPoint = contactPoint.point;
        Debug.Log("Collision point is: " + collisionPoint);

        Vector3 collisionNormal = contactPoint.normal;

        Bounce(collisionNormal);

    }

    private void Bounce(Vector3 colNormal)
    {
        Vector3 beforeImpactVelocity = shipRB.Velocity;
        Vector3 reflection = Vector3.Reflect(beforeImpactVelocity, colNormal);
        Debug.Log("relection direction is: " + reflection);
        shipRB.AddForce((reflection * 500f), ForceMode.Impulse);

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
                float torqueStrength = angle * 2f; // Adjust torque strength as needed

                // Apply torque to rotate around the axis
                shipRB.AddTorque(rotationAxis * torqueStrength, ForceMode.Force);
            }
        }
    }
}
