using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWorld : MonoBehaviour
{
    //child OBJ that handles the positon
    [SerializeField] Transform childPositionOBJ;

    //checks whether the player is controlling the ship
    [SerializeField] 
    public bool playerControllingShip;


    //floats for the player inputs
    public float AirshipYaw;
    public float AirshipThrust;
    public float AirshipRoll;
    public float AirshipPitch;
    public float AirshipAscend;

    //controls the rotation and movespeed of the airship
    [SerializeField] public float rotSpeed = 10f;
    [SerializeField] public float moveSpeed = 5f;


    private void Start()
    {
        //checks to make sure that positioning isnt null if it is fills it
        if (childPositionOBJ == null)
        {childPositionOBJ = transform.GetChild(0).gameObject.transform;}
    }
    private void Update()
    {
        //checks that the player is actively controlling the ship
        //TODO might need tinkering depedning on how networking works
        if (playerControllingShip)
        {
            ShipInputs();
            RotateWorld();
            MoveWorld();
        }
        
    }

    /// <summary>
    /// Gets all the input from the player and assigns them
    /// </summary>
    void ShipInputs()
    {
        Debug.Log(AirshipYaw);
        //Debug.Log("cum cum beans");
        //AirshipYaw = Input.GetAxisRaw("Horizontal");
        //AirshipThrust = Input.GetAxisRaw("Vertical");

        //AirshipRoll = Input.GetAxisRaw("AirshipRoll");
        //AirshipPitch = Input.GetAxisRaw("AirshipPitch");

        //AirshipAscend = Input.GetAxisRaw("AirshipAscend");
    }

    /// <summary>
    /// rotates the enviroment based on player inputs
    /// </summary>
    void RotateWorld()
    {
        float yawDirection = AirshipYaw * Time.deltaTime *-1;
        float rollDirection = AirshipRoll * Time.deltaTime*-1;
        float pitchDirection = AirshipPitch * Time.deltaTime*-1;



        transform.RotateAround(Vector3.zero, Vector3.up, yawDirection * rotSpeed);
        transform.RotateAround(Vector3.zero, Vector3.forward, rollDirection * rotSpeed);
        transform.RotateAround(Vector3.zero, Vector3.right, pitchDirection * rotSpeed);


    }

    /// <summary>
    /// Updates position of the childOBJ based on player inputs
    /// </summary>
    void MoveWorld()
    {
        //get the floats valeus for the player inputs for the w/s for forwardDirection and z/x for ascendDirection
        float forwardDirection = AirshipThrust * Time.deltaTime;
        float ascendDirection = AirshipAscend * Time.deltaTime *-1;

        //moves the postion parent which should contain the enviroment
        childPositionOBJ.transform.position = new Vector3(childPositionOBJ.transform.position.x, childPositionOBJ.transform.position.y+ (ascendDirection * moveSpeed), childPositionOBJ.transform.position.z + (forwardDirection * moveSpeed));

    }


}
