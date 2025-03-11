using GLTFast.Schema;
using Gravitas;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.Shapes;

public class SCR_ShipControls : NetworkBehaviour
{
    public bool onWheel = false;

    [SerializeField]
    public GameObject ship;
    public SCR_ShipMovement shipMovement;

    [SerializeField] private Animator wheelAnimator;
    private GameObject[] wheelPiecesToRotate;
    private GameObject[] wheelRimPieces;
    private GameObject wheelCentrePost;

    private void Start()
    {
        Invoke("LoadShip", 1);
    }

    private void LoadShip()
    {
        ship = GameObject.Find("PRE-Airship");
        shipMovement = ship.GetComponent<SCR_ShipMovement>();
        wheelAnimator = GameObject.Find("Wheel").GetComponent<Animator>();
        wheelPiecesToRotate = GameObject.FindGameObjectsWithTag("WheelRotatePieces");
        wheelRimPieces = GameObject.FindGameObjectsWithTag("WheelRimPieces");
        wheelCentrePost = GameObject.Find("WheelCentrePost");
    }


    private void Update()
    {
        if (ship == null) { LoadShip(); return; }
        if (shipMovement == null) { if (ship != null) { LoadShip(); } }
        if (!IsOwner) { return; }
        if (!onWheel) { return; }

        if (Input.GetKeyDown(KeyCode.LeftShift) && shipMovement.shipAcceleration.Value < shipMovement.shipAccelerationBound)
        {
            shipMovement.increaseAccelerationServerRPC();
            Debug.Log("speed up: " + shipMovement.shipAcceleration.Value);
        }

        if (Input.GetKeyDown(KeyCode.LeftControl) && shipMovement.shipAcceleration.Value > -shipMovement.shipAccelerationBound/2)
        {
            shipMovement.decreaseAccelerationServerRPC();
            Debug.Log("speed down: " + shipMovement.shipAcceleration.Value);
        }

        //Yaw Right
        if (Input.GetKey(KeyCode.D) == true)
        {
            Debug.Log("Turning right");
            shipMovement.updateYawRotServerRPC("Right", OwnerClientId);
            float centreZRotation = NormalizeAngle(wheelCentrePost.transform.eulerAngles.z);
            if (centreZRotation <= -179.5f)
            {
                Debug.Log("Stopping wheel rotation to the right");
                return;
            }
            foreach (GameObject wheelPiece in wheelPiecesToRotate)
            {
                Debug.Log("Turning wheel pieces right");
                wheelPiece.transform.Rotate(new Vector3(0,0,-1), 0.5f);
                Vector3 wheelRotation = wheelCentrePost.transform.eulerAngles;
                Debug.Log("wheel centre post rotation is: " + wheelRotation);
                //wheelPiece.transform.localRotation = Quaternion.Euler(wheelPiece.transform.localRotation.x, wheelPiece.transform.localRotation.x + 1.0f, wheelPiece.transform.localRotation.z);
                //wheelPiece.transform.Rotate(wheelPiece.transform.rotation.x, wheelPiece.transform.rotation.y + 1.0f, wheelPiece.transform.rotation.z);
            }
            foreach(GameObject wheelRimPiece in wheelRimPieces)
            {
                wheelRimPiece.transform.Rotate(new Vector3(0, 1, 0), 0.5f);
            }
            //wheelAnimator.speed = 1;
            //wheelAnimator.SetBool("TurningLeft", false);
            //wheelAnimator.SetBool("TurningRight", true);
        }
        //Yaw Left
        if (Input.GetKey(KeyCode.A) == true)
        {
            Debug.Log("Turning left");
            shipMovement.updateYawRotServerRPC("Left", OwnerClientId);
            float centreZRotation = NormalizeAngle(wheelCentrePost.transform.eulerAngles.z);
            if (centreZRotation >= 179.5f)
            {
                Debug.Log("Stopping wheel rotation to the left");
                return;
            }
            foreach (GameObject wheelPiece in wheelPiecesToRotate)
            {
                Debug.Log("Turning wheel pieces left");
                wheelPiece.transform.Rotate(new Vector3(0, 0, 1), 0.5f);
                //wheelPiece.transform.localRotation = Quaternion.Euler(wheelPiece.transform.localRotation.x, wheelPiece.transform.localRotation.x - 1.0f, wheelPiece.transform.localRotation.z);
                //wheelPiece.transform.Rotate(wheelPiece.transform.rotation.x, wheelPiece.transform.rotation.y - 1.0f, wheelPiece.transform.rotation.z);
            }
            foreach (GameObject wheelRimPiece in wheelRimPieces)
            {
                wheelRimPiece.transform.Rotate(new Vector3(0, -1, 0), 0.5f);
            }
            //wheelAnimator.speed = 1;
            //wheelAnimator.SetBool("TurningLeft", true);
            //wheelAnimator.SetBool("TurningRight", false);
        }
        if(Input.GetKey(KeyCode.D) == false && Input.GetKey(KeyCode.A) == false) 
        {
            wheelAnimator.SetBool("Turning", false);
            wheelAnimator.speed = 0;
            wheelAnimator.SetBool("TurningLeft", false);
            wheelAnimator.SetBool("TurningRight", false);
            wheelAnimator.SetBool("NotTurning", true);
        }
        //Roll Right
        if (Input.GetKey(KeyCode.E) && !(ship.transform.rotation.eulerAngles.x > 180 && ship.transform.rotation.eulerAngles.x < 360 - shipMovement.autoCorrectLimit))
        {

            if (ship.transform.rotation.eulerAngles.x < 180)
            {
                shipMovement.updateRollRotServerRPC("Left", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updateRollRotServerRPC("Left", OwnerClientId);
            }


            shipMovement.autoLevelRollActive = false;
            CancelInvoke(nameof(startAutoLevelRoll));
            Invoke(nameof(startAutoLevelRoll), 0.5f);
        }
        //Roll Left
        if (Input.GetKey(KeyCode.Q) && !(ship.transform.rotation.eulerAngles.x < 180 && ship.transform.rotation.eulerAngles.x > shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.x > 180)
            {
                shipMovement.updateRollRotServerRPC("Right", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updateRollRotServerRPC("Right", OwnerClientId);
            }

            shipMovement.autoLevelRollActive = false;
            CancelInvoke(nameof(startAutoLevelRoll));
            Invoke(nameof(startAutoLevelRoll), 0.5f);
        }
        //Pitch up
        if (Input.GetKey(KeyCode.S) && !(ship.transform.rotation.eulerAngles.z < 180 && ship.transform.rotation.eulerAngles.z > shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.z > 180)
            {
                shipMovement.updatePitchRotServerRPC("Right", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updatePitchRotServerRPC("Right", OwnerClientId);
            }

            shipMovement.autoLevelPitchActive = false;
            CancelInvoke(nameof(startAutoLevelPitch));
            Invoke(nameof(startAutoLevelPitch), 0.5f);
        }
        //Pitch Down
        if (Input.GetKey(KeyCode.W) && !(ship.transform.rotation.eulerAngles.z > 180 && ship.transform.rotation.eulerAngles.z < 360 - shipMovement.autoCorrectLimit))
        {
            if (ship.transform.rotation.eulerAngles.z < 180)
            {
                shipMovement.updatePitchRotServerRPC("Left", OwnerClientId, 2f);
            }
            else
            {
                shipMovement.updatePitchRotServerRPC("Left", OwnerClientId);
            }

            shipMovement.autoLevelPitchActive = false;
            CancelInvoke(nameof(startAutoLevelPitch));
            Invoke(nameof(startAutoLevelPitch), 0.5f);
        }
    }

    // Update is called once per frame
    //void FixedUpdate()
    //{
    //    if (ship == null) { ship = GameObject.Find("PRE-Airship"); return; }
    //    if (shipMovement == null) { if (ship != null) { shipMovement = ship.GetComponent<SCR_ShipMovement>(); } }
    //    if (!IsOwner){ return; }
    //    if (!onWheel) { return; }



    //}

    private float NormalizeAngle(float angle)
    {
        if (angle > 180) angle -= 360;
        return angle;
    }

    private void startAutoLevelRoll()
    {
        shipMovement.autoLevelRollActive = true;
    }
    private void startAutoLevelPitch()
    {
        shipMovement.autoLevelPitchActive = true;
    }
}
