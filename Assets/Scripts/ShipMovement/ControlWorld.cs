using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlWorld : MonoBehaviour
{
    [SerializeField] Transform worldCentrePosition;
    [SerializeField] Transform PlatformDirection;
    [SerializeField] bool PlayerControllingShip;

    [SerializeField] bool IncludeRoll;


    private float horizontalInput;
    private float verticalInput;
    private float AirshipRoll;
    private float AirshipPitch;

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
        if (PlayerControllingShip)
        {
            ShipInputs();
            RotateWorld();
            MoveWorld();
        }
        
    }

    void ShipInputs()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        AirshipRoll = Input.GetAxisRaw("AirshipRoll");
        AirshipPitch = Input.GetAxisRaw("AirshipPitch");
    }

    void RotateWorld()
    {

        //version with multiple gameobjects
        //Rotates along the y axis the world *-1 for world rotation so it has to be reversed
        float yawDirection = horizontalInput * Time.deltaTime*-1;
        float rollDirection = AirshipRoll * Time.deltaTime;
        float pitchDirection = AirshipPitch * Time.deltaTime;



        Vector3 RotationTotal = new Vector3(pitchDirection, yawDirection, rollDirection);

        //this.transform.rotation = Quaternion.Euler(RotationTotal * rotSpeed + this.transform.rotation.eulerAngles);
        transform.RotateAround(worldCentrePosition.position, Vector3.up, yawDirection * rotSpeed);
        transform.RotateAround(worldCentrePosition.position, Vector3.forward, rollDirection * rotSpeed);
        transform.RotateAround(worldCentrePosition.position, Vector3.right, pitchDirection * rotSpeed);






    }

    void MoveWorld()
    {
        //Vector3 InvertedTransform = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * -1);
        //moves the world along the z axis forward and backwards *-1 to reverse for world rotation
        float direction = verticalInput * Time.deltaTime *-1;
        this.transform.position = new Vector3(0, 0, direction * moveSpeed+ this.transform.position.z);
        //worldCentrePosition.transform.position = (direction * moveSpeed) + worldCentrePosition.transform.position;
        //worldCentrePosition.transform.localPosition = (InvertedTransform * direction * moveSpeed) + worldCentrePosition.transform.localPosition;
    }
}
