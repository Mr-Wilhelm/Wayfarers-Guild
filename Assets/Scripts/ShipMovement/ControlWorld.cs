using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWorld : MonoBehaviour
{
    [SerializeField] Transform worldCentrePosition;
    //[SerializeField] Transform PlatformDirection;
    [SerializeField] bool PlayerControllingShip;

    [SerializeField] Vector3 OffsetPos;

    [SerializeField] bool IncludeRoll;


    private float horizontalInput;
    private float verticalInput;
    private float AirshipRoll;
    private float AirshipPitch;
    private float AirshipAscend;

    [SerializeField] float rotSpeed = 10f;
    [SerializeField] float moveSpeed = 5f;


    private void Start()
    {
        if (worldCentrePosition == null)
        {
            worldCentrePosition = transform.GetChild(0).gameObject.transform;
        }
    }
    private void Update()
    {
        ShipInputs();
        RotateWorld();
        MoveWorld();
    }

    void ShipInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        AirshipRoll = Input.GetAxisRaw("AirshipRoll");
        AirshipPitch = Input.GetAxisRaw("AirshipPitch");

        AirshipAscend = Input.GetAxisRaw("AirshipAscend");
    }

    void RotateWorld()
    {
        //Rotates along the y axis the world *-1 for world rotation so it has to be reversed
        float yawDirection = horizontalInput * Time.deltaTime *-1;
        float rollDirection = AirshipRoll * Time.deltaTime*-1;
        float pitchDirection = AirshipPitch * Time.deltaTime*-1;



        Vector3 RotationTotal = new Vector3(pitchDirection, yawDirection, rollDirection);

        Vector3 offsetPos = this.transform.position*-1;
        OffsetPos = offsetPos;
        //this.transform.rotation = Quaternion.Euler(RotationTotal * rotSpeed + this.transform.rotation.eulerAngles);

        transform.RotateAround(Vector3.zero, Vector3.up, yawDirection * rotSpeed);
        transform.RotateAround(Vector3.zero, Vector3.forward, rollDirection * rotSpeed);
        transform.RotateAround(Vector3.zero, Vector3.right, pitchDirection * rotSpeed);

        //transform.Translate(new Vector3(yawDirection, rollDirection, pitchDirection * rotSpeed)+this.transform.rotation.eulerAngles);




    }

    void MoveWorld()
    {
        Vector3 InvertedTransform = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * -1 );
        //moves the world along the z axis forward and backwards *-1 to reverse for world rotation


        float forward = verticalInput * Time.deltaTime;
        float Up = AirshipAscend * Time.deltaTime;
        //this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, direction * moveSpeed+ this.transform.position.z);


        //worldCentrePosition.transform.position = (direction * moveSpeed) + worldCentrePosition.transform.position;
        //worldCentrePosition.transform.position = (InvertedTransform * direction * moveSpeed) + worldCentrePosition.transform.position;
        worldCentrePosition.transform.position = new Vector3(worldCentrePosition.transform.position.x, worldCentrePosition.transform.position.y+ (Up * moveSpeed), worldCentrePosition.transform.position.z + (forward * moveSpeed));


    }


}
