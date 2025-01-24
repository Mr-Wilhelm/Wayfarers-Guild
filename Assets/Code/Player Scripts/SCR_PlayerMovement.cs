using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.VisualScripting;

public class SCR_PlayerMovement : NetworkBehaviour
{
    public float moveSpeed;
    public Transform orientation;
    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;
    Rigidbody rb;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [SerializeField] GameObject Ship = null;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;  [Header("Movement")]


    [Header("Ground Check")]
    public float groundDrag;

    public float playerHeight;
    public LayerMask isGround;
    bool grounded;

    [Header("Interaction")]


    [SerializeField]
    private SCR_PlayerInteract playerInteractScript;

    [SerializeField]
    private Camera cam1;

    [SerializeField]
    private Camera cam2;

    [SerializeField]
    private bool camsAreSwitched;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        playerInteractScript = gameObject.GetComponent<SCR_PlayerInteract>();
        cam1 = GameObject.Find("Camera").GetComponent<Camera>();
        cam2 = GameObject.Find("Camera2").GetComponent<Camera>();

    }

    private void Update()
    {
        
    }
    
    // Update is called once per frame
    void FixedUpdate()
    {
        GroundCheck();
        if (!IsOwner) { enabled = false; }
        MyInput();
        SpeedControl();

        if(cam2 == null)
        {
            cam2 = GameObject.Find("Camera2").GetComponent<Camera>();
        }

        if(camsAreSwitched)
        {
            cam2.enabled = true;
            cam1.enabled = false;
        }
        else
        {
            cam2.enabled = false;
            cam1.enabled = true;
        }

        if (!playerInteractScript.isInteracting)
        {
            MovePlayer();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            camsAreSwitched = true;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            camsAreSwitched = false;
        }
    }
    


    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        //moveDirection = (verticalInput * orientation.right + horizontalInput * orientation.forward * -1);
        if (grounded)
        {
            rb.AddRelativeForce(moveDirection.normalized * moveSpeed * 10, ForceMode.Force);
        }
        else
        {
            rb.AddRelativeForce(moveDirection.normalized * moveSpeed * 10 * airMultiplier, ForceMode.Force);
        }
    }

    private void GroundCheck()
    {
        //Uncoment to add ground check
        //grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, isGround);

        //if (grounded) rb.drag = groundDrag;
        //else rb.drag = 0f;

        rb.drag = groundDrag;
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, ((orientation.forward * 2) + this.transform.position));
    }
}
