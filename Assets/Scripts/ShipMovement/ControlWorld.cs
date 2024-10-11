using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlWorld : MonoBehaviour
{
    [SerializeField] GameObject worldCentrePosition;
    [SerializeField] Transform PlatformDirection;
    [SerializeField] bool PlayerControllingShip;

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
    }

    void RotateWorld()
    {
        //Rotates along the y axis the world *-1 for world rotation so it has to be reversed
        float direction = horizontalInput * Time.deltaTime *-1;
        float rollDirection = AirshipRoll * Time.deltaTime;
        float pitchDirection = AirshipPitch * Time.deltaTime;
        this.transform.rotation = Quaternion.Euler(new Vector3(pitchDirection, direction, rollDirection) * rotSpeed + this.transform.rotation.eulerAngles);

    }
    void MoveWorld()
    {
        Vector3 InvertedTransform = new Vector3(transform.forward.x, transform.forward.y, transform.forward.z * -1);
        //moves the world along the z axis forward and backwards *-1 to reverse for world rotation
        float direction = verticalInput * Time.deltaTime;
        worldCentrePosition.transform.localPosition = (InvertedTransform*direction* moveSpeed)+worldCentrePosition.transform.localPosition;
    }
}
