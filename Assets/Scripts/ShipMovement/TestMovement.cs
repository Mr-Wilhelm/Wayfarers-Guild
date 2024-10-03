using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TestMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    [SerializeField] Vector3 moveDirection;
    Rigidbody rb;

    [SerializeField]
    float FrictionStrength = 0.4f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Update()
    {
    }
    void FixedUpdate()
    {
        MovePlayer();
        MyInput();
        FakeFriction();
    }
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
    }
    private void MovePlayer()
    {
        
        moveDirection = (verticalInput * Vector3.right + horizontalInput* Vector3.forward*-1);
        //rb.AddForce(moveDirection, ForceMode.Force);
        //moveDirection = (orientation.right *verticalInput + (orientation.forward*-1) * horizontalInput).normalized;
        rb.AddRelativeForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);

    }

    private void FakeFriction()
    {
        rb.velocity = Time.deltaTime * rb.velocity * FrictionStrength;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, ((orientation.right * 2) + this.transform.position));
    }
}
