using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipVelocity : MonoBehaviour
{
    [SerializeField] float maxVelocity = 10f;
    [SerializeField] float speed = 3f;
    [SerializeField] float rotSpeed = 3f;
    [SerializeField] bool playerControllingShip = false;
    [SerializeField] float currentVelocity;

    private float verticalInput;
    private float horizontalInput;

    [SerializeField] Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (playerControllingShip)
        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            AddVelocityToShip();
            RotateShip();
        }
        currentVelocity = rb.velocity.z;
    }

    private void AddVelocityToShip()
    {
        rb.velocity = verticalInput * this.transform.forward*Time.deltaTime*speed + rb.velocity;
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }
    private void RotateShip()
    {
        float direction = horizontalInput * Time.deltaTime;

        //this.transform.rotation = Quaternion.Euler(0f,direction,0f);

        this.transform.rotation = Quaternion.Euler(new Vector3(0f,direction * rotSpeed, 0f)+ this.transform.rotation.eulerAngles);

        

    }
}
