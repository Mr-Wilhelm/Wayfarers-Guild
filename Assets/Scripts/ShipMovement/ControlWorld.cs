using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ControlWorld : MonoBehaviour
{
    [SerializeField] GameObject worldCentrePosition;
    [SerializeField] Transform PlatformDirection;
    [SerializeField] bool PlayerControllingShip;

    [SerializeField] bool IncludeRoll;
 
    [SerializeField] Transform WorldRotationX;
    [SerializeField] Transform WorldRotationY;
    [SerializeField] Transform WorldRotationZ;

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
            worldCentrePosition = transform.GetChild(0).gameObject;
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
        Vector3 MultiplyingForce = this.transform.rotation.eulerAngles;

        //matrrices need to be done here
        //this.transform.rotation = Quaternion.Euler(RotationTotal * rotSpeed + this.transform.rotation.eulerAngles);
        WorldRotationX.localRotation = Quaternion.Euler(new Vector3 (pitchDirection * rotSpeed + WorldRotationX.localRotation.eulerAngles.x, 0,0));
        WorldRotationY.localRotation = Quaternion.Euler(new Vector3 (0,yawDirection * rotSpeed + WorldRotationY.localRotation.eulerAngles.y, 0));



        if (IncludeRoll)
        {
            WorldRotationZ.localRotation = Quaternion.Euler(new Vector3(0, 0, rollDirection * rotSpeed + WorldRotationZ.localRotation.eulerAngles.z));

        }

    }
    void MoveWorld()
    {
        Vector3 InvertedTransform = new Vector3(worldCentrePosition.transform.forward.x, worldCentrePosition.transform.forward.y, worldCentrePosition.transform.forward.z * -1);
        //moves the world along the z axis forward and backwards *-1 to reverse for world rotation
        float direction = verticalInput * Time.deltaTime;
        worldCentrePosition.transform.localPosition = (InvertedTransform*direction* moveSpeed)+worldCentrePosition.transform.localPosition;
    }
}
